#I "../packages"
#r @"FSharp.Data/lib/net40/FSharp.data.dll"

open FSharp.Data
open System
let key = "your key to the Guardian API"

(*
Retrieving headlines from the "World" section
in the Guardian newspaper, with the JSON type provider
*)

// http://open-platform.theguardian.com/explore/

[<Literal>]
let sampleUrl = "http://content.guardianapis.com/search?section=world&api-key=test"

// TODO create a type Headlines
// Show recent headlings

type Headlines = JsonProvider<sampleUrl>
let sample = Headlines.GetSample ()

sample.Response.Results
|> Seq.iter (fun p ->  printfn "%s" p.WebTitle)


(*
Using a minimal F# DSL to pull headlines
between user-defined dates.
*)

let fromDate (date:DateTime) query = query + "&from-date=" + date.ToString("yyyy-MM-dd")
let toDate (date:DateTime) query = query + "&to-date=" + date.ToString("yyyy-MM-dd")
let pageSize (size:int) query = query + "&page-size=" + (size.ToString())
let withKey key query = query + "&api-key=" + key

let baseUrl = "http://content.guardianapis.com/search?section=world"

let run startDate endDate page =
    baseUrl
    |> fromDate startDate
    |> toDate endDate
    |> pageSize page
    |> withKey key
    |> Headlines.Load

let testRequest = run DateTime.Now DateTime.Now 20

testRequest.Response.Results
|> Seq.iter (fun article -> printfn "%s"  article.WebTitle)


(*
Using m-brace to download and store
one year of Guardian headlines.
*)

#load "AzureCluster.fsx"

open System
open System.IO
open MBrace.Core
open MBrace.Flow

let cluster = Config.GetCluster()

(*
Creating our entire data file, entirely in the cloud
Never touches my machine
*)

let filePath = "cluster/guardian"
//CloudFile.Delete(filePath) |> cluster.RunLocally

cloud {
    let headlines =
        seq {
            let startDate = DateTime(2014,11,17)
            for day in 0 .. 364 do
                let date = startDate.AddDays(day |> float)
                printfn "processing %A" date
                let response = run date date 50
                let results = response.Response.Results
                for result in results ->
                    result.WebPublicationDate, result.WebTitle
                // I know, I know, this is awful
                // but there is a throttle on the service...
                System.Threading.Thread.Sleep 100
            }
        |> Seq.map (fun (date,title) ->
            sprintf "%s|%s" (date.ToShortDateString()) title)

    return! CloudFile.WriteAllLines(filePath,headlines) }
    |> cluster.Run


(*
Introducing CloudFlow
*)

// TODO: how many headlines did we pull?

filePath
|> CloudFlow.OfCloudFileByLine
|> CloudFlow.length
|> cluster.Run

// TODO: print 5 lines

filePath
|> CloudFlow.OfCloudFileByLine
|> CloudFlow.take 5
|> CloudFlow.toArray
|> cluster.Run
|> Array.iter (printfn "%s")

// TODO: count lines > 50 chars
// TODO: WithDegreeOfParallelism

filePath
|> CloudFlow.OfCloudFileByLine
|> CloudFlow.withDegreeOfParallelism 8
|> CloudFlow.filter (fun x -> x.Length > 50)
|> CloudFlow.length
|> cluster.Run


(*
Consuming and running C# code

*)

// TODO show C# library

#r @"../TextAnalyzer/bin/Debug/TextAnalyzer.dll"
open TextAnalyzer

Analyzer.Words("the QuICK brown FOX fox FOX")
Analyzer.WordsCount("the QuICK brown FOX fox FOX")

// TODO: identify unique words in Headlines

#time "on"

// for illustration: 10 first headlines
filePath
|> CloudFlow.OfCloudFileByLine
|> CloudFlow.take 10
|> CloudFlow.map (fun x -> Analyzer.Words(x)) // calling C# dll
|> CloudFlow.toArray
|> cluster.Run
|> Array.map (Set.ofArray)
|> Set.unionMany

// extract only correctly formed dates/headlines
let parseLine (line:string) =
    if line.Length = 0 then None
    else
        let blocks = line.Split '|'
        try
            let date = blocks.[0] |> Convert.ToDateTime
            let title = blocks.[1]
            Some(date,title)
        with _ -> None

// extract unique words
let words =
    filePath
    |> CloudFlow.OfCloudFileByLine
    |> CloudFlow.withDegreeOfParallelism 16
    |> CloudFlow.choose (parseLine)
    |> CloudFlow.map (fun (date,title) -> Analyzer.Words(title))
    |> CloudFlow.map (fun words -> words |> Set.ofArray)
    |> CloudFlow.toArray
    |> cluster.Run
    |> Set.unionMany

// compute word count
let wordCount =
    filePath
    |> CloudFlow.OfCloudFileByLine
    |> CloudFlow.withDegreeOfParallelism 16
    |> CloudFlow.choose (parseLine)
    |> CloudFlow.map (fun (date,title) -> Analyzer.WordsCount(title))
    |> CloudFlow.collect id
    |> CloudFlow.groupBy (fun wc -> wc.Word)
    |> CloudFlow.map (fun (word,counts) -> word, counts |> Seq.sumBy (fun x -> x.Count))
    |> CloudFlow.toArray
    |> cluster.Run
    |> dict

// TODO: most frequent words?

(*
The raw word count is not very interesting:
if I visualize day-by-day, I will see a lot
of "and", "the", "in"...

A better way to find "interesting words" that
are specific for a day is to use tf-idf:
count how often a word is present in a day,
and divide by how frequent that word is overall.
*)

let wordCountByDay =
    filePath
    |> CloudFlow.OfCloudFileByLine
    |> CloudFlow.withDegreeOfParallelism 16
    |> CloudFlow.choose parseLine
    |> CloudFlow.groupBy (fun (date,title) -> date)
    |> CloudFlow.map (fun (date,group) ->
        let dayCount =
            group
            |> Seq.map snd
            |> String.concat " "
            |> Analyzer.WordsCount
        date,dayCount)
    |> CloudFlow.toArray
    |> cluster.Run

let minimum = 10
let documents =
    filePath
    |> CloudFlow.OfCloudFileByLine
    |> CloudFlow.length
    |> cluster.Run

let summary =
    wordCountByDay
    |> Array.map (fun (date,words) ->
        let day =
            words
            |> Seq.filter (fun wc -> wordCount.[wc.Word] > minimum)
            |> Seq.map (fun wc ->
                let total = wordCount.[wc.Word]
                let tfidf = float wc.Count * (float documents / float total)
                (wc.Word,tfidf))
            |> Seq.map (fun (word,tfidf) -> sprintf "%s,%.3f" word tfidf)
            |> String.concat "|"
        sprintf "%s|%s" (date.ToShortDateString()) day)


let path = __SOURCE_DIRECTORY__ + "/summary"
System.IO.File.WriteAllLines(path,summary)
