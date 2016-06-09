// An Experiment

#load "dependencies.fsx"
open Dependencies

open HomeDepot.Core
open HomeDepot.Features

#r @"Accord/lib/net45/Accord.dll"
#r @"Accord.Math/lib/net45/Accord.Math.dll"
#r @"Accord.Statistics/lib/net45/Accord.Statistics.dll"

open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Fitting

#load "logistic.fs"
open HomeDepot.Logistic

let model = [|
    ``search length``
    ``product title length``
    ``matching words``
    |]

let logPredictor = logistic model sample     

sample
|> Array.averageBy (fun (label,obs) -> 
    abs (label - logPredictor obs))
