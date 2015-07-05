// Requires reference to 
// PresentationCore, PresentationFramework, 
// System.Windows.Presentation, System.Xaml, WindowsBase

#r "PresentationCore.dll"
#r "PresentationFramework.dll"
#r "System.Windows.Presentation.dll"
#r "System.Xaml.dll"
#r "WindowsBase.dll"

open System
open System.Windows
open System.Windows.Media
open System.Windows.Shapes
open System.Windows.Controls

let data =
    __SOURCE_DIRECTORY__ + "/summary"
    |> System.IO.File.ReadAllLines
    |> Array.map (fun line -> line.Split '|')
    |> Array.map (fun line ->
        let date = line.[0] |> Convert.ToDateTime
        let data = 
            line.[1..]
            |> Array.map (fun x -> x.Split ',')
            |> Array.map (fun x -> x.[0], float x.[1])
            |> Map.ofArray
        date,data)
    |> Map.ofArray

let maxValue = 
    data 
    |> Seq.map (fun x -> 
        x.Value 
        |> Seq.map (fun y -> y.Value)
        |> Seq.max)
    |> Seq.max

let words =
    data
    |> Seq.map (fun kv -> kv.Value |> Seq.map (fun x -> x.Key))
    |> Seq.collect id
    |> Seq.distinct
    |> Seq.toArray


let createBlock (target:Canvas) (text:string) (size:float) (pos:float*float) =
    let textBlock = TextBlock()
    textBlock.Text <- text
    textBlock.FontFamily <- FontFamily("Arial Black")
    textBlock.FontSize <- size
    textBlock.Foreground <- Brushes.Red
    target.Children.Add(textBlock) |> ignore 
    let top,left = pos
    Canvas.SetTop(textBlock, top)
    Canvas.SetLeft(textBlock, left)
    textBlock

let win = new Window()
let canvas = new Canvas()
canvas.Background <- Brushes.Black

let width = 1200
let height = 800
let margin = 100

let rng = Random ()

let blocks =
    words
    |> Seq.map (fun word -> 
        let size = 1.
        let top = rng.Next(margin, int height - margin) |> float
        let left = rng.Next(margin, int width - margin) |> float
        word, createBlock canvas word size (top,left))
    |> Map.ofSeq

let sizes = blocks |> Map.map (fun word _ -> 0.)

let byDay =
    __SOURCE_DIRECTORY__ + "/summary"
    |> System.IO.File.ReadAllLines
    |> Array.map (fun line -> line.Split '|')
    |> Array.map (fun line ->
        let date = line.[0] |> Convert.ToDateTime
        let data = 
            line.[1..]
            |> Array.map (fun x -> x.Split ',')
            |> Array.map (fun x -> x.[0], float x.[1])
            |> Array.filter (fun (w,c) -> blocks.ContainsKey w)
            |> Map.ofArray
        date,data)
    |> Map.ofArray

let dateblock = createBlock canvas "" 40. (10.,10.)

win.Content <- canvas
win.WindowState <- WindowState.Maximized
win.Show()

let updateSizes (sizes:Map<string,float>) (dayData:Map<string,float> option) =
    match dayData with
    | None -> sizes |> Map.map (fun k v -> v * 0.9)
    | Some(data) ->
        sizes
        |> Map.map (fun word oldIntensity ->
            let newIntensity =
                match data.TryFind word with
                | None -> 0.
                | Some(s) -> s
            max newIntensity (0.85 * oldIntensity + 0.5 * newIntensity))


let dispatcher = win.Dispatcher

let startDate = DateTime(2014,1,1)

let rec loop (day:int,sizes:Map<string,float>) = 
    async {
        do! Async.Sleep 100

        let date = startDate.AddDays(day |> float)
        let dayData = byDay.TryFind date

        let newSizes = updateSizes sizes dayData
        
        dispatcher.Invoke(
            fun _ ->
                dateblock.Text <- date.ToShortDateString ()
                // update display
                blocks
                |> Seq.iter (fun kv ->
                    let block = kv.Value
                    let size = newSizes.[kv.Key]
                    let size = size / maxValue
                    let brush = SolidColorBrush(Colors.Gold)
                    brush.Opacity <- size
                    block.Foreground <- brush
                    if size > 0.01 
                    then block.FontSize <- 200. * size))
                      
        return! loop (day + 1, newSizes) }

loop (0,sizes) |> Async.Start

[<STAThread()>]
do 
   let app =  new Application() in
   app.Run() |> ignore

