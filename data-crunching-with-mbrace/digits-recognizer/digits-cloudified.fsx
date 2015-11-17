#load "AzureCluster.fsx"

open System
open System.IO
open MBrace.Core
open MBrace.Flow

// TODO: Runtime, Logger, Workers, Processes

let cluster = Config.GetCluster()
cluster.ShowWorkers ()
cluster.ShowProcesses ()

// TODO: cloud hello

let localHello name = sprintf "Hello, %s" name

localHello "#fsharp"

let asyncHello name = 
    async { return sprintf "Hello, %s" name } 

asyncHello "#fsharp" |> Async.RunSynchronously

let cloudHello name = 
    cloud { return sprintf "Hello, %s" name }

cloudHello "#fsharp" |> cluster.Run

// TODO: run, create process

let job = cloudHello "#fsharp" |> cluster.CreateProcess

job.Status
job.Result

// TODO: copy/paste/update digits-local.fsx code

// this remains unchanged, as it should

open System.IO

type Image = int[]
type Example = { Label:int; Pixels:Image }

let distance (img1:Image) (img2:Image) =
    let mutable acc = 0
    let dim = max img1.Length img2.Length
    for i in 0 .. (dim - 1) do
        let diff = img1.[i] - img2.[i]
        acc <- acc + abs diff
    acc

let predictor (sample:Example[]) (k:int) (img:Image) =
    sample
    |> Seq.sortBy (fun ex -> distance ex.Pixels img)
    |> Seq.take k
    |> Seq.countBy (fun ex -> ex.Label)
    |> Seq.maxBy (fun (label,count) -> count)
    |> fst

let trainingPath = __SOURCE_DIRECTORY__ + "/trainingsample.csv"
let validationPath = __SOURCE_DIRECTORY__ + "/validationsample.csv"

let parseLine (line:string) =
    let cols = line.Split(',')
    let label = cols.[0] |> int
    let pixels = cols.[1..] |> Array.map int
    { Label = label; Pixels = pixels }

let dropHeaders (xs:_[]) = xs.[1..]

// we need to upload the data to the cluster

let cloudTrainingPath = "cluster/trainingsample.csv"
let cloudValidationPath = "cluster/validationsample.csv"

let cloudTrain =
    CloudFile.Upload(trainingPath,cloudTrainingPath)
    |> cluster.RunLocally

let cloudTest =
    CloudFile.Upload(validationPath,cloudValidationPath)
    |> cluster.RunLocally

// we can now run our evaluation in the cluster

let cloudCorrect =
    cloud {
        let! train = CloudFile.ReadAllLines(cloudTrain.Path)
        let! test = CloudFile.ReadAllLines(cloudTest.Path)

        let training =
            train
            |> dropHeaders
            |> Array.map parseLine
        
        let testing = 
            test
            |> dropHeaders
            |> Array.map parseLine

        let correct =
            let model = predictor training 1
            testing
            |> Array.averageBy (fun ex ->
                if model ex.Pixels = ex.Label then 1.0 else 0.0)
            
        return correct }

// run the evaluation

let evaluation = cloudCorrect |> cluster.CreateProcess
evaluation.Status
evaluation.Result

(*
This is somewhat useful: I can use
a large machine now instead of my laptop.

What would be really useful though
is running jobs in parallel...

For example, take k = 1, 2, .. 10 and
tell me which one is the best.
*)

// TODO pick k

let parallelized = cloud {
    let! train = CloudFile.ReadAllLines(cloudTrain.Path)
    let! test = CloudFile.ReadAllLines(cloudTest.Path)

    let train = train.[1..] |> Array.map parseLine
    let test = test.[1..] |> Array.map parseLine

    let! correct = 
        [ for k in 1 .. 10 -> 
            cloud { 
                let model = predictor train k
                let correctForK =
                    test
                    |> Array.averageBy (fun ex ->
                        if model ex.Pixels = ex.Label then 1.0 else 0.0)
                return (k,correctForK) } ]
        |> Cloud.Parallel
    return correct }

let result = parallelized |> cluster.CreateProcess
result.Status

(*
Rendering evaluation for k = 1, .. 10,
using F# Charting
*)

#load "../packages/FSharp.Charting/fsharp.charting.fsx"
open FSharp.Charting

result.Result |> Chart.Line
