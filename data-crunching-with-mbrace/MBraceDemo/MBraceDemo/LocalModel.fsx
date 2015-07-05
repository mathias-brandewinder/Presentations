(*
A purely local model, to predict what number
an image represents. The dataset is a subset
of the Kaggle digit recognizer competition:
https://www.kaggle.com/c/digit-recognizer
*)

// Create a basic k-nearest-neighbor classifier

type Image = int []
type Observation = { Label:int; Pixels:Image }
type Distance = Image -> Image -> int

// manhattan distance between 2 images:
// compute the difference, pixel by pixel
// and sum up the absolute values.
let manhattan (img1:Image) (img2:Image) =
    let dim = max (img1.Length) (img2.Length)
    let mutable d = 0
    for i in 0 .. (dim - 1) do
        d <- d + abs (img1.[i] - img2.[i])
    d

// knn classifier: 
// given an image, find the k closest images
// in the sample observations, and return the
// the majority label.
let knnClassifier (distance:Distance) (sample:Observation[]) k (image:Image) =
    sample
    // sort the sample by distance
    |> Seq.sortBy (fun example -> distance example.Pixels image) 
    // keep the k closest
    |> Seq.take k
    // count by labels
    |> Seq.countBy (fun example -> example.Label)
    // find the group with most frequent label
    |> Seq.maxBy (fun (label,count) -> count)
    // return the label
    |> fst

// read the training and validation files in memory

let toObservation (line:string) =
    line.Split ','
    |> Array.map int
    |> fun x -> { Label = x.[0]; Pixels = x.[1..] }

open System.IO

let readFile path =
    let lines = File.ReadAllLines(path)
    lines.[1..]
    |> Array.map toObservation

let trainingPath = __SOURCE_DIRECTORY__ + "/trainingsample.csv"
let validationPath = __SOURCE_DIRECTORY__ + "/validationsample.csv"

// evaluate the quality of 1-nearest-neighbor:
// what proportion of images from the validation
// sample do we predict correctly?

// activate the timer
#time "on"

let evaluate = 
    let training = readFile trainingPath
    // create a prediction model
    let model = knnClassifier manhattan training 1
    // create an array of Observation from the validation set
    let validation = readFile validationPath
    // compute the % correctly predicted
    let correct = 
        validation 
        |> Array.averageBy (fun ex -> 
            if model ex.Pixels = ex.Label then 1.0 else 0.0)
    correct

// at that point, we would like to search for
// the "best model"; for instance, we could
// search for which k = 1, 2, ... gives us the
// best predictions. Or we could try out other
// distances. This is expensive - and can be 
// run in parallel.
