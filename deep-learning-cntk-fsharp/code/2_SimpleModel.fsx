(*
    Loading the library into the scripting environment
*)

#I "./packages/CNTK.FSharp/"
#load "scripts/Dependencies.fsx"
#r "lib/CNTK.FSharp.dll"

open CNTK
open CNTK.FSharp

open System.IO
open System.Collections.Generic

// look at the dataset: slightly noisy version of 8 x 8 images

// let one = 
//     [
//         "0 0 0 0 0 0 0 0"  
//         "0 0 0 0 1 0 0 0"  
//         "0 0 0 1 1 0 0 0"  
//         "0 0 1 0 1 0 0 0"  
//         "0 0 0 0 1 0 0 0"  
//         "0 0 0 0 1 0 0 0"  
//         "0 0 1 1 1 1 1 0"  
//         "0 0 0 0 0 0 0 0"         
//     ]

// goal: distinguish between 
// images that are a 1, and 
// images that are a 2 (not a 1)


(*
    Model specification
    Output = pixel[0,0] * weight[0,0] + ... + bias
*)

let input = Variable.InputVariable(shape [8 * 8], DataType.Float)
let weights = new Parameter(shape [1; 8 * 8], DataType.Float, CNTKLib.GlorotUniformInitializer())
let constant = new Parameter(shape [1], DataType.Float, 0.0)

let model = 
    let product = CNTKLib.Times(weights,input)
    CNTKLib.Plus(var product, constant) 
    |> var 
    |> CNTKLib.Sigmoid

// CNTKLib. : standard operations

let label = Variable.InputVariable(shape [1], DataType.Float)

(*
    Setting up learning
*)

let loss = CNTKLib.SquaredError(var model, label)
let eval = CNTKLib.SquaredError(var model, label)

let learningRate = new TrainingParameterScheduleDouble(0.001, uint32 1)

let parameterLearners =
    ResizeArray<Learner>(
        [ 
            Learner.SGDLearner(model.Parameters(), learningRate) 
        ])
      
let trainer = Trainer.CreateTrainer(model, loss, eval, parameterLearners)

(*
    Setting up data reads
*)

let streamConfigurations = 
    ResizeArray<StreamConfiguration>(
        [
            new StreamConfiguration("pixels", 8 * 8)    
            new StreamConfiguration("labels", 1)
        ]
        )

let minibatchSource = 
    MinibatchSource.TextFormatMinibatchSource(
        Path.Combine(__SOURCE_DIRECTORY__, "images"), 
        streamConfigurations, 
        MinibatchSource.InfinitelyRepeat)

let featureStreamInfo = minibatchSource.StreamInfo("pixels")
let labelStreamInfo = minibatchSource.StreamInfo("labels")

let device = DeviceDescriptor.CPUDevice
let minibatchData = minibatchSource.GetNextMinibatch(64u, device)

let arguments : IDictionary<Variable, MinibatchData> =
    [
        input, minibatchData.[featureStreamInfo]
        label, minibatchData.[labelStreamInfo]
    ]
    |> dict

(*
    Doing 1000 passes of learning over the dataset
*)

for _ in 1 .. 1000 do

    trainer.TrainMinibatch(arguments, device) |> ignore

    trainer
    |> Minibatch.summary 
    |> Minibatch.basicPrint

(*
    Visualize the weights the model learnt
*)

let learnedWeights = 

    let inputDataMap = 
        [
            input, minibatchData.[featureStreamInfo].data
        ]
        |> dataMap

    let computedWeights = 
        model.Parameters () 
        |> Seq.filter (fun p -> 64 = (p.Shape.Dimensions |> Seq.fold (*) 1))
        |> Seq.head

    let outputDataMap = 
        [ 
            computedWeights :> Variable, null 
        ] 
        |> dataMap
        
    model.Evaluate(inputDataMap, outputDataMap, device)

    let learnedWeights = 
        outputDataMap.[computedWeights].GetDenseData<float32>(computedWeights)
        |> Seq.map (fun ws -> ws |> Seq.map float |> Seq.toArray)
        |> Seq.toArray
        |> Array.collect id

    learnedWeights

#load "Visualizer.fsx"
open Visualizer

learnedWeights
|> Visualizer.showWeights 25
