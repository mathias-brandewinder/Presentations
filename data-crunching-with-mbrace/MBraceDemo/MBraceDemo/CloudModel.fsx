(*
Let's take the model we created in LocalModel.fsx,
but use mbrace to run the model on a cluster,
and search for the best model in a parallel fashion.
*)

// make sure your connection strings
// are correctly setup in Credentials.fsx

#load "Credentials.fsx"

open MBrace.Core
open MBrace.Azure
open MBrace.Azure.Client
open MBrace.Flow
open MBrace.Store

// connect to the cluster
let cluster = Runtime.GetHandle(config)

// attach a logger
cluster.AttachClientLogger(ConsoleLogger())

// check workers & processes
cluster.ShowWorkers ()
cluster.ShowProcesses ()

// creating & running a job 
let job = cloud { return "hello" } 
job |> cluster.Run

(*
Transforming the original model into a
version that can be run in the cluster.
*)

// The first part of the model has no reason to change:
// we can re-use the code from LocalModel.fsx

type Image = int []
type Observation = { Label:int; Pixels:Image }
type Distance = Image -> Image -> int

let manhattan (img1:Image) (img2:Image) =
    let dim = max (img1.Length) (img2.Length)
    let mutable d = 0
    for i in 0 .. (dim - 1) do
        d <- d + abs (img1.[i] - img2.[i])
    d

let knnClassifier (distance:Distance) (sample:Observation[]) k (image:Image) =
    sample
    |> Seq.sortBy (fun example -> distance example.Pixels image) 
    |> Seq.take k
    |> Seq.countBy (fun example -> example.Label)
    |> Seq.maxBy (fun (label,count) -> count)
    |> fst

let toObservation (line:string) =
    line.Split ','
    |> Array.map int
    |> fun x -> { Label = x.[0]; Pixels = x.[1..] }

// we will move our data to the cluster, into cloud
// storage, to make data access more efficient.

let trainingPath = __SOURCE_DIRECTORY__ + "/trainingsample.csv"
let validationPath = __SOURCE_DIRECTORY__ + "/validationsample.csv"

// upload the 2 CSV files to cloud storage

let training = 
    CloudFile.Upload(trainingPath, "data/training") 
    |> cluster.RunLocally

let validation = 
    CloudFile.Upload(trainingPath, "data/validation") 
    |> cluster.RunLocally

// we modify the readFile function
// to operate on CloudFile, instead 
// of (local) File originally
let readFile path = cloud {
    let! lines = CloudFile.ReadAllLines(path)
    let sample = 
        lines.[1..]
        |> Array.map toObservation
    return sample }

// evaluate the quality of 1-nearest-neighbor,
let evaluate = cloud {
    let! training = readFile (training.Path)
    let model = knnClassifier manhattan training 1
    let! validation = readFile (validation.Path)
    let correct = 
        validation 
        |> Array.averageBy (fun ex -> 
            if model ex.Pixels = ex.Label then 1.0 else 0.0)
    return correct }

// launch the evaluation (non-blocking)
let evaluation = evaluate |> cluster.CreateProcess
// is this done yet?
evaluation.Completed
// if yes, retrieve the result
evaluation.AwaitResult ()


// more interesting: evaluate for k = 1 .. 10
let bestModel = cloud {
    let ``quality for k`` =
        [ 1 .. 10 ]
        |> List.map (fun k ->
            // create a separate process for each k
            cloud {
                let! training = readFile (training.Path)
                let! validation = readFile (validation.Path)
                // create a model for k
                let model = knnClassifier manhattan training k
                let correct = 
                    validation 
                    |> Array.averageBy (fun ex -> 
                        if model ex.Pixels = ex.Label then 1.0 else 0.0)
                // return k and the % correct
                return (k, correct) } )
        |> Cloud.Parallel // run all these in parallel
        |> cluster.Run

    return ``quality for k`` }

let searchBestModel = bestModel |> cluster.CreateProcess
// is this done yet?
searchBestModel.Completed
// if yes, retrieve the result
let results = searchBestModel.AwaitResult ()

// let's plot the results for each k

#I @"../packages"
#load @"FSharp.Charting/FSharp.Charting.fsx"
open FSharp.Charting

results |> Chart.Line