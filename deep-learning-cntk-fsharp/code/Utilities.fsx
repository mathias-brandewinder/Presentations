#I "./packages/CNTK.FSharp/"
#load "scripts/Dependencies.fsx"
#r "lib/CNTK.FSharp.dll"

open CNTK
open CNTK.FSharp
open CNTK.FSharp.Sequential
 
open System.IO

let indexOfLargest xs = 
    let largest = xs |> Seq.max
    xs |> Seq.findIndex ((=) largest) 

let evaluate 
    (modelFile:string)
    (textSource:TextFormatSource)
    (device:DeviceDescriptor) =

        let model = Function.Load(modelFile, device)

        // can this whole block be extracted into a function?
        let imageInput = 
            model.Inputs
            |> Seq.filter (fun i -> i.IsInput)
            |> Seq.exactlyOne

        let labelOutput = model.Output

        let mappings : TextFormat.InputMappings = {
            Features = [ 
                { Variable = imageInput; SourceName = textSource.Features }
                ] 
            Labels = { Variable = labelOutput; SourceName = textSource.Labels }
            }

        let testMinibatchSource = TextFormat.source (textSource.FilePath, mappings)

        let featureStreamInfo = testMinibatchSource.StreamInfo(textSource.Features)
        let labelStreamInfo = testMinibatchSource.StreamInfo(textSource.Labels)

        let batchSize = 50

        let rec countErrors (total,errors) =

            printfn "Total: %i; Errors: %i" total errors

            let minibatchData = testMinibatchSource.GetNextMinibatch((uint32)batchSize, device)

            if (minibatchData = null || minibatchData.Count = 0)
            then (total,errors)        
            else

                let total = total + minibatchData.[featureStreamInfo].numberOfSamples

                // find the index of the largest label value
                let labelData = 
                    minibatchData
                    |> Minibatch.getValues testMinibatchSource textSource.Labels
                    |> Minibatch.getDense labelOutput

                let expectedLabels = 
                    labelData 
                    |> Seq.map indexOfLargest

                // compute the predicted label
                let inputDataMap = 
                    [
                        imageInput, minibatchData.[featureStreamInfo].data
                    ]
                    |> dataMap

                let outputDataMap = 
                    [ 
                        labelOutput, null 
                    ] 
                    |> dataMap
                    
                model.Evaluate(inputDataMap, outputDataMap, device)
                
                let outputData = 
                    outputDataMap.[labelOutput]
                    |> Minibatch.getDense labelOutput 
                
                let actualLabels =
                    outputData 
                    |> Seq.map indexOfLargest

                let misMatches = 
                    (actualLabels,expectedLabels)
                    ||> Seq.zip
                    |> Seq.sumBy (fun (a, b) -> if a = b then 0 else 1)

                let errors = errors + misMatches

                if Minibatch.isSweepEnd (minibatchData)
                then (total,errors)
                else countErrors (total,errors)

        countErrors (uint32 0,0)

let predict(
    modelFile:string, 
    textSource:TextFormatSource,
    device:DeviceDescriptor
    ) =

        let model = Function.Load(modelFile, device)

        let imageInput = 
            model.Inputs
            |> Seq.filter (fun i -> i.IsInput)
            |> Seq.exactlyOne

        let labelOutput = model.Output

        let mappings : TextFormat.InputMappings = {
            Features = [ 
                { Variable = imageInput; SourceName = textSource.Features }
                ] 
            Labels = { Variable = labelOutput; SourceName = textSource.Labels }
            }

        let testMinibatchSource = TextFormat.source (textSource.FilePath, mappings)

        let featureStreamInfo = testMinibatchSource.StreamInfo(textSource.Features)
        let labelStreamInfo = testMinibatchSource.StreamInfo(textSource.Labels)

        let batchSize = 20

        let minibatchData = testMinibatchSource.GetNextMinibatch((uint32)batchSize, device)

        let labelData = 
            minibatchData
            |> Minibatch.getValues testMinibatchSource textSource.Labels
            |> Minibatch.getDense labelOutput

        let expectedLabels = 
            labelData 
            |> Seq.map indexOfLargest

        // retrieve also the pixels, as a flat array
        let pixelsInput = 
            Variable.InputVariable(shape[28*28], DataType.Float)

        // compute the predicted label
        let inputDataMap = 
            [
                imageInput, minibatchData.[featureStreamInfo].data
                pixelsInput, minibatchData.[featureStreamInfo].data
            ]
            |> dataMap

        let outputDataMap = 
            [ 
                labelOutput, null 
            ] 
            |> dataMap
            
        model.Evaluate(inputDataMap, outputDataMap, device)
        
        let pixels = 
            inputDataMap.[pixelsInput]
            |> Minibatch.getDense(pixelsInput)
        
        let outputData = 
            outputDataMap.[labelOutput]
            |> Minibatch.getDense labelOutput 
        
        let predictedLabels = 
            outputData
            |> Seq.map indexOfLargest

        (pixels,expectedLabels,predictedLabels) 
        |||> Seq.zip3 
