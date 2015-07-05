(*
One of the limitations of the local/cloud example 
is that we started with data on our local machine.
This won't do if our dataset is large.
In this example, our goal is to work with data
without ever bringing it down to the local machine,
so that all the processing happens in the cluster.
The data is small enough that we could do it 
entirely locally, but illustrates ideas that would
work with "big" data.
*)

// using the JSON type provider to access the Guardian API

#I "../packages"
#r @"FSharp.Data/lib/net40/FSharp.Data.dll"
open FSharp.Data

// create a type by inspecting the response from the Guardian API
// check the link below for more on the API:
// http://open-platform.theguardian.com/explore/

[<Literal>]
let sampleUrl = "http://content.guardianapis.com/search?section=world&api-key=test"
type Headlines = JsonProvider<sampleUrl>

// inspect what we get
let sample = Headlines.GetSample ()
sample.Response.Results 
|> Seq.iter (fun result -> printfn "%s" result.WebTitle)

// a tiny DSL to compose requests
open System

let fromDate (date:DateTime) query = query + "&from-date=" + date.ToString("yyyy-MM-dd")
let toDate (date:DateTime) query = query + "&to-date=" + date.ToString("yyyy-MM-dd")
let pageSize (size:int) query = query + "&page-size=" + (size.ToString())
let withKey key query = query + "&api-key=" + key

let baseUrl = "http://content.guardianapis.com/search?section=world"

let key = "test" // replace by a proper Guardian API key

let headlines startDate endDate page =
    baseUrl
    |> fromDate startDate
    |> toDate endDate
    |> pageSize page
    |> withKey key
    |> Headlines.Load
    |> fun response -> response.Response.Results

// example usage: print 20 of today's headlines
let today = 
    headlines (DateTime.Now) (DateTime.Now) 20
    |> Seq.iter (fun h -> printfn "%s" h.WebTitle)

#load "credentials.fsx"

open MBrace.Core
open MBrace.Azure
open MBrace.Azure.Client
open MBrace.Client
open MBrace.Store
open MBrace.Flow

let cluster = Runtime.GetHandle(config)
cluster.AttachClientLogger(ConsoleLogger())

(* 
Let's create a datafile, entirely in the cloud,
retrieving every headline for 2014.
*)

let filePath = "data/guardian"

let saveHeadlines = cloud {
    let headlines = 
        seq {
            let startDate = DateTime(2014,1,1)
            for day in 0 .. 364 do
                let date = startDate.AddDays(day |> float)
                let results = headlines date date 50
                // extract date and headlines
                for result in results ->
                    result.WebPublicationDate, result.WebTitle
                // artificially slow down processing
                // because the API has a throttle :(
                System.Threading.Thread.Sleep 100    
            }
        |> Seq.map (fun (date,title) -> 
            sprintf "%s|%s" (date.ToShortDateString()) title)
    
    return! CloudFile.WriteAllLines(filePath,headlines) }

saveHeadlines |> cluster.CreateProcess

(*
With CloudFlow, we can process the file
remotely, in a fashion similar to Seq.
*)

// how many headlines do we have?
CloudFile(filePath).Path
|> CloudFlow.OfCloudFileByLine
|> CloudFlow.length
|> cluster.Run

// grab 5 headlines and return them as an array
CloudFile(filePath).Path
|> CloudFlow.OfCloudFileByLine
|> CloudFlow.take 5
|> CloudFlow.toArray
|> cluster.Run

(*
Consuming and running C# code:
see the TextAnalyzer project included.
*)

#r @"../TextAnalyzer/bin/Debug/TextAnalyzer.dll"
open TextAnalyzer
Analyzer.Words("the QuICK brown FOX fox FOX")
Analyzer.WordsCount("Developers, developers... developers!")

// Identify unique words in the Guardian headlines

#time "on"

let guardian = CloudFile(filePath)

let parseLine (line:string) =
    let blocks = line.Split '|'
    try
        let date = blocks.[0] |> Convert.ToDateTime
        let title = blocks.[1]
        Some(date,title)
    with _ -> None

// find unique words     
let words = 
    guardian.Path
    |> CloudFlow.OfCloudFileByLine
    // run operation in parallel
    |> CloudFlow.withDegreeOfParallelism 16
    // some filtering to avoid empty entries
    |> CloudFlow.filter (fun line -> line.Length > 0)
    |> CloudFlow.map (parseLine)
    |> CloudFlow.filter (fun x -> x.IsSome)
    |> CloudFlow.map (fun x -> x.Value)
    // Use C# code to extract words as a Set
    |> CloudFlow.map (fun (date,title) -> Analyzer.Words(title))
    |> CloudFlow.map (Set.ofArray)
    |> CloudFlow.toArray
    |> cluster.Run
    // union of all uniques found
    |> Set.unionMany

// run a word count on all the headlines
let wordCount = 
    guardian.Path
    |> CloudFlow.OfCloudFileByLine
    |> CloudFlow.withDegreeOfParallelism 16
    |> CloudFlow.filter (fun line -> line.Length > 0)
    |> CloudFlow.map (parseLine)
    |> CloudFlow.filter (fun x -> x.IsSome)
    |> CloudFlow.map (fun x -> x.Value)
    |> CloudFlow.map (fun (date,title) -> Analyzer.WordsCount(title))
    |> CloudFlow.collect id
    |> CloudFlow.groupBy (fun wordcount -> wordcount.Word)
    |> CloudFlow.map (fun (word,counts) -> word, counts |> Seq.sumBy (fun x -> x.Count))
    |> CloudFlow.toArray
    |> cluster.Run

// 10 most frequent words?
wordCount
|> Array.sortBy (fun (word,count) -> - count)
|> Seq.take 10
|> Seq.iter (fun (word,count) -> printfn "%s,%i" word count)

(*
The raw word count is not very interesting:
if I visualize day-by-day, I will see a lot
of "and", "the", "in"...

A better way to find "interesting words" that
are specific for a day is to use tf-idf:
count how often a word is present in a day,
and divide by how frequent a word is overall.
For more details, see for instance:
http://nlp.stanford.edu/IR-book/html/htmledition/inverse-document-frequency-1.html
*)

// for each day of the years, group headlines
// and produce a wordcount for that day.
let wordCountByDay =
    guardian.Path
    |> CloudFlow.OfCloudFileByLine
    |> CloudFlow.withDegreeOfParallelism 16
    |> CloudFlow.filter (fun line -> line.Length > 0)
    |> CloudFlow.map (parseLine)
    |> CloudFlow.filter (fun x -> x.IsSome)
    |> CloudFlow.map (fun x -> x.Value)
    |> CloudFlow.groupBy (fun (date,title) -> date)
    |> CloudFlow.map (fun (date,group) ->
        // quick-and-dirty: flatten all the headlines
        // for a day into one string, and perform a
        // word count on that concatenated headline.
        let dayCount = 
            group
            |> Seq.map snd
            |> String.concat " "
            |> Analyzer.WordsCount
        date,dayCount)
    |> CloudFlow.toArray
    |> cluster.Run

// For each word, compute the idf, 
// the inverse document frequency.
// Words present every day will have
// a low idf, rare ones a high idf
let idfs =
    let documents = wordCountByDay.Length
    words
    |> Seq.map (fun word ->
        let occurences = 
            wordCountByDay
            |> Array.filter (fun (day,counts) -> 
                counts |> Array.exists (fun wc -> wc.Word = word))
            |> Array.length
        let idf = log (float documents / float occurences)
        word,idf)
    |> dict

// push overall word count in a dictionary 
// for efficient access by word
let wordCountDictionary = wordCount |> dict

// Create a summary: filter out the very rare words
// and the very common ones, and for each day
// compute the tf-idf for each of the remaining words.
let summary =
    wordCountByDay
    |> Array.map (fun (date,words) ->
        let totalWords = words |> Seq.sumBy (fun wc -> wc.Count)
        words 
        |> Array.filter (fun wc -> wordCountDictionary.[wc.Word] > 5)
        |> Array.filter (fun wc -> idfs.[wc.Word] > 0.1)
        |> Array.map (fun wc -> 
            let tf = float wc.Count / float totalWords
            let idf = idfs.[wc.Word]
            wc.Word, tf * idf)
        |> Array.map (fun (word,tfidf) -> sprintf "%s,%.3f" word tfidf)
        |> String.concat "|"
        |> fun words -> sprintf "%s|%s" (date.ToShortDateString()) words)

// the file "summary" is the result I got;
// you can now run it in the Animation.fsx script
let path = __SOURCE_DIRECTORY__ + "/summary"
System.IO.File.WriteAllLines(path,summary)