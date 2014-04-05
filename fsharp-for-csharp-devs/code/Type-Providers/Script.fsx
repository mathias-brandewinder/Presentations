(*
Note: for this script to run properly,
you need to have R installed.
*)

#I @"..\packages\"
#r @"FSharp.Data.2.0.5\lib\net40\FSharp.Data.dll"
#r @"RProvider.1.0.5\lib\RDotNet.dll"
#r @"RProvider.1.0.5\lib\RProvider.dll"

open System
open FSharp.Data
open RDotNet
open RProvider
open RProvider.``base``
open RProvider.graphics

type Titanic = CsvProvider<"Titanic.csv">
let passengers = (new Titanic()).Rows
let first = passengers |> Seq.head

// we get access to live data from the World Bank,
// with IntelliSense for discoverability
let wb = WorldBankData.GetDataContext()
let capital = wb.Countries.Afghanistan.CapitalCity

// We can now work against R, with Intellisense...
let rng = Random()
let data = R.c([| for i in 0 .. 1000 -> rng.NextDouble() |])
R.plot(data)
R.hist(data |> R.log)

// ... and grab live data from the World Bank,
// and send it to R for visualization
let countries = [|
    wb.Countries.Canada;
    wb.Countries.``United States``;
    wb.Countries.Mexico;
    wb.Countries.Brazil;
    wb.Countries.Argentina;
    wb.Countries.``United Kingdom``;
    wb.Countries.France;
    wb.Countries.Germany;
    wb.Countries.``South Africa``;
    wb.Countries.Kenya;
    wb.Countries.``Russian Federation``;
    wb.Countries.China;
    wb.Countries.Japan;
    wb.Countries.Australia |]

let gdp2000 = countries |> Array.map (fun c -> c.Indicators.``GDP (current US$)``.[2000])
let gdp2010 = countries |> Array.map (fun c -> c.Indicators.``GDP (current US$)``.[2010])

let series = [
    "GDP2000", gdp2000;
    "GDP2010", gdp2010; ]

let dataframe = R.data_frame(namedParams series)
R.plot(dataframe)

let names = countries |> Array.map (fun c -> c.Name)
R.text(gdp2000, gdp2010, names)