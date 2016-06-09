// dependencies

#I @"../packages/"
#r @"FSharp.Data/lib/net40/FSharp.Data.dll"
#r @"Accord/lib/net45/Accord.dll"
#r @"Accord.Math/lib/net45/Accord.Math.dll"
#r @"Accord.Statistics/lib/net45/Accord.Statistics.dll"

open FSharp.Data

open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Fitting


// Core model

type Observation = {
    Search:string
    Product:string }

type Predictor = Observation -> float

type Feature = Observation -> float

type Example = float * Observation

type Model = Feature []

type Learning = Model -> Example [] -> Predictor


// Algorithm adapter

let logistic : Learning =
 
    fun model ->
        fun sample ->

            // put data in correct shape
            // for the specific algorithm implementation
            let featurize (obs:Observation) =
                model 
                |> Array.map (fun f -> f obs)
                
            let normalize x = x - 2.0
            let denormalize x = x + 2.0

            let labels, features = 
                sample
                |> Array.map (fun (label,obs) ->
                    normalize label,
                    featurize obs)
                |> Array.unzip

            // learn relationship between input/output

            let inputsCount = model.Length
            let regression = LogisticRegression(inputsCount)
            let teacher = IterativeReweightedLeastSquares(regression)

            let rec learn () =
                let error = teacher.Run(features, labels)
                if error < 0.01 
                then regression
                else learn ()

            let predictor : Predictor =
                fun obs ->
                    let features = featurize obs
                    regression.Compute features
                    |> denormalize

            // return predictor
            predictor


// Catalog of Features


let ``search length`` : Feature =
    fun obs -> obs.Search.Length |> float
    
let ``product title length`` : Feature = 
    fun obs -> obs.Product.Length |> float

let ``matching words`` : Feature =
    fun obs ->
        let search = obs.Search.Split ' ' |> set
        let product = obs.Product.Split ' ' |> set
        Set.intersect search product |> Set.count |> float


// Actual experiment
let model = [|
    ``search length``
    ``product title length``
    |]

// learn a logistic

type Train = CsvProvider<"train.csv">
let sample = 
    Train.GetSample().Rows 
    |> Seq.map (fun row ->
        row.Relevance |> float,
        {
            Search = row.Search_term
            Product = row.Product_title
        })
    |> Seq.toArray

let logPredictor = logistic model sample     

sample
|> Array.averageBy (fun (label,obs) -> 
    abs (label - logPredictor obs))


// add a feature?

// one simplification: FeatureLearner

// Show actual example in our code

// 1. Features are separate in one file
// 2. Script = model (combo of features) + algo
// 3. Easy to add/remove a feature
// 4. Note: not object oriented at all: feature = function


