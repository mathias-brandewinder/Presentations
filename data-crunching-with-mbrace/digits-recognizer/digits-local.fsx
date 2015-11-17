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

let train =
    trainingPath
    |> File.ReadAllLines
    |> dropHeaders
    |> Array.map parseLine

let test = 
    validationPath
    |> File.ReadAllLines
    |> dropHeaders
    |> Array.map parseLine

#time "on"

let correct =
    let model = predictor train 1
    test
    |> Array.averageBy (fun ex ->
        if model ex.Pixels = ex.Label then 1.0 else 0.0)
