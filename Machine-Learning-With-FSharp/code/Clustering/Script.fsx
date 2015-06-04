open System.IO

let datapath = __SOURCE_DIRECTORY__ + @"\iris.data"

type Observation = { 
    SepalLength: float
    SepalWidth: float
    PetalLength: float
    PetalWidth: float
    Class: string }

let data =
    File.ReadAllLines(datapath)
    |> fun lines -> lines.[1..]
    |> Array.map (fun line -> line.Split(','))
    |> Array.map (fun line -> 
        {   SepalLength = line.[0] |> float;
            SepalWidth = line.[1] |> float;
            PetalLength = line.[2] |> float;
            PetalWidth = line.[3] |> float;
            Class = line.[4]; })

// visualize the data

#load "..\packages\FSharp.Charting.0.90.10\Fsharp.Charting.fsx"
open FSharp.Charting












data
|> Seq.groupBy (fun obs -> obs.Class)
|> Seq.map (fun (species,group) -> 
    group |> Seq.map (fun obs -> obs.PetalLength, obs.PetalWidth))
|> Seq.map Chart.Point
|> Seq.map (fun chart -> chart.WithMarkers(Size = 10))
|> Chart.Combine


(* k-means implementation *)

#load "KMeans.fs"
open Unsupervised.KMeans

// note: not a single interface...

// define distance & reducer

let distance obs1 obs2 =
    (obs1.SepalWidth - obs2.SepalWidth) ** 2. +
    (obs1.SepalLength - obs2.SepalLength) ** 2. +
    (obs1.PetalWidth - obs2.PetalWidth) ** 2. +
    (obs1.PetalLength - obs2.PetalLength) ** 2.

let reducer observations =
    { SepalWidth = observations |> Seq.averageBy (fun p -> p.SepalWidth);
      SepalLength = observations |> Seq.averageBy (fun p -> p.SepalLength);
      PetalWidth = observations |> Seq.averageBy (fun p -> p.PetalWidth);
      PetalLength = observations  |> Seq.averageBy (fun p -> p.PetalLength);
      Class = "Centroid" }

let (clusters, classifier) = clusterize distance reducer data 3

// plot results...

data
|> Seq.groupBy classifier
|> Seq.map (fun (predicted,group) -> 
    group |> Seq.map (fun obs -> obs.PetalLength, obs.PetalWidth))
|> Seq.map Chart.Point
|> Seq.map (fun chart -> chart.WithMarkers(Size = 10))
|> Chart.Combine

// change distance?







let manhattan p1 p2 =
    abs (p1.SepalWidth - p2.SepalWidth)  +
    abs (p1.SepalLength - p2.SepalLength) +
    abs (p1.PetalWidth - p2.PetalWidth) +
    abs (p1.PetalLength - p2.PetalLength)

let (clusters2, classifier2) = clusterize manhattan reducer data 3


data
|> Seq.groupBy classifier2
|> Seq.map (fun (predicted,group) -> 
    group |> Seq.map (fun obs -> obs.PetalLength, obs.PetalWidth))
|> Seq.map Chart.Point
|> Seq.map (fun chart -> chart.WithMarkers(Size = 10))
|> Chart.Combine