namespace HomeDepot

// Adapter around the algorithm
module Logistic =

    open HomeDepot.Core
    open Accord.Statistics.Models.Regression
    open Accord.Statistics.Models.Regression.Fitting

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

                learn ()
                
                let predictor : Predictor =
                    fun obs ->
                        let features = featurize obs
                        regression.Compute features
                        |> denormalize

                // return predictor
                predictor
