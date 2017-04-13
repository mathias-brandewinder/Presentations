#r "System.Net.Http.dll"

open System
open System.Net
open System.Net.Http
open System.Text
open Newtonsoft.Json

#load "../Shared.fsx"
open Shared

let prepare (msg:string) =
    sprintf """{"text":"%s"}""" msg
    |> fun txt -> StringContent(txt, Encoding.UTF8)

let Run(inputMessage: string, log: TraceWriter) =

    let inputMessage = JsonConvert.DeserializeObject<Message> (inputMessage)
    let inputMessage = inputMessage.Rate

    let client = new HttpClient()
    let url = "https://hooks.slack.com/services/YOUR-KEY-GOES-HERE"
    let msg = prepare inputMessage
    
    client.PostAsync(url,msg) |> ignore

    log.Info(sprintf "F# Queue trigger function processed: '%s'" inputMessage)
