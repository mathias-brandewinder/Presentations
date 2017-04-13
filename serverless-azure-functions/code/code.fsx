(*
    PART 1: PULLING EXCHANGE RATES FROM YAHOO
*)

open System.Net

let url = """http://query.yahooapis.com/v1/public/yql?q=select * from yahoo.finance.xchange where pair in ("GBPUSD")&env=store://datatables.org/alltableswithkeys"""

let client = new WebClient()
let result = client.DownloadString(url)
printfn "%s" result

(*
    PART 2: POSTING TO SLACK
*)

#r "System.Net.Http.dll"

open System
open System.Net
open System.Net.Http
open System.Text

let client = new HttpClient()
let url = "https://hooks.slack.com/services/YOUR-KEY-GOES-HERE"
let msg = new StringContent("""{"text":"bayazure"}""", Encoding.UTF8)

client.PostAsync(url,msg) |> ignore
