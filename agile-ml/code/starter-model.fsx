// take a look at the data first

(*
Our starting point: perhaps relevance
depends on 
- length of Search Terms,
- length of Product Title,
- Search term longer than average (1/0),
- matching chars, words, ... 
*)


// 1. Loading data

#I @"../packages/"
#r @"FSharp.Data/lib/net40/FSharp.Data.dll"

open FSharp.Data

type sample = CsvProvider<"train.csv">


// 2. Extract features as vectors

let output, input = 
    sample.GetSample().Rows
    |> Seq.map (fun row ->
        row.Relevance |> float,
        [|
            row.Search_term.Length |> float
            row.Product_title.Length |> float
        |])
    |> Seq.toArray
    |> Array.unzip


// 3. Use some algorithm to learn
// Here, logistic regression

#r @"Accord/lib/net45/Accord.dll"
#r @"Accord.Math/lib/net45/Accord.Math.dll"
#r @"Accord.Statistics/lib/net45/Accord.Statistics.dll"

open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Fitting

// massage data to work with the algorithm
let normalizedOutput = 
    output 
    |> Array.map (fun x -> (x - 1.0) / 2.0)

let inputsCount = 2
let model = LogisticRegression(inputsCount)
let teacher = LogisticGradientDescent(model)

let rec learn iter =
    let error = teacher.Run(input, normalizedOutput)
    printfn "%.3f" error
    if iter = 0 
    then model
    else learn (iter - 1)

let regression = learn 100


// 4. check how good/bad the model is
// (Simplified: should hold separate data)

let predictor (obs:sample.Row) =
    [| 
        obs.Search_term.Length |> float
        obs.Product_title.Length |> float
    |]
    |> regression.Compute
    |> fun x -> (x * 2.0) + 1.0

// Compute RMSE / quality metric 
sample.GetSample().Rows
|> Seq.averageBy (fun row ->
    pown ((row.Relevance |> float) - (predictor row)) 2)
|> sqrt


(*
What next?

It is unlikely that our model is perfect,
so we will change it, and create different
experiments.

What are we going to change?
- add, remove, edit features? 
- use a different algorithm?

What will hurt?
- code duplication in features declaration
- swapping algoritms
- very hard to see what the features are

We want to
- extract features so they can be reused

What is always the same
- the data, and reading it
- the evaluation / model structure
*)
