#I @"../packages/"
#r @"FSharp.Data/lib/net45/FSharp.Data.dll"
#load "core-model.fs"
#load "catalog-of-features.fs"

open HomeDepot.Core
open HomeDepot.Features
open FSharp.Data

type Train = CsvProvider<"train.csv">

// common data loading for all experiments
let sample = 
    Train.GetSample().Rows 
    |> Seq.map (fun row ->
        row.Relevance |> float,
        {
            Search = row.Search_term
            Product = row.Product_title
        })
    |> Seq.toArray
