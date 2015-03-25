(*
Note: for this script to run properly,
you need to have R installed.
*)

#I @"..\packages\"
#r @"FSharp.Data.2.2.0\lib\net40\FSharp.Data.dll"
#r @"R.NET.Community.1.5.16\lib\net40\RDotNet.dll"
#r @"RProvider.1.1.8\lib\net40\RProvider.Runtime.dll"
#r @"RProvider.1.1.8\lib\net40\RProvider.dll"

open System
open FSharp.Data
open RDotNet
open RProvider
open RProvider.``base``
open RProvider.graphics


type Titanic = CsvProvider<"Titanic.csv">
let passengers = (new Titanic()).Rows
let first = passengers |> Seq.head


type TopUsers = JsonProvider<"""https://api.stackexchange.com/2.2/tags/F%23/top-answerers/all_time?site=stackoverflow""">
let sample = TopUsers.GetSample ()
sample.Items |> Seq.iter (fun user -> printfn "%s" user.User.DisplayName)


// we get access to live data from the World Bank,
// with IntelliSense for discoverability
let wb = WorldBankData.GetDataContext()
wb.Countries.Canada.CapitalCity


open RProvider.graphics

// We can now work against R, with Intellisense...
let rng = Random()
let data = [ for i in 0 .. 1000 -> rng.NextDouble() ]
R.plot(data)
R.hist(data |> List.map (fun x -> x * x))

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

(* This whole section requires the R rworldmap package
to be installed (it is not part of the standard distribution).
*)

open RProvider.rworldmap

let df =
    let codes, pops = 
        query { for country in wb.Countries -> 
                    country.Code, 
                    country.Indicators.``Population, total``.[2010]/country.Indicators.``Population, total``.[2000] }
        |> Seq.toArray
        |> Array.unzip
    let data =
        [ "Code", codes |> box 
          "Pop", pops |> box ]
    R.data_frame (namedParams data)

let map = R.joinCountryData2Map(df, "ISO3", "Code")
R.mapDevice()
R.mapCountryData(map,"Pop")