#r @"..\packages\R.NET.Community.1.5.16\lib\net40\RDotNet.dll"
#r @"..\packages\RProvider.1.1.8\lib\net40\RProvider.dll"
#r @"..\packages\RProvider.1.1.8\lib\net40\RProvider.Runtime.dll"
#r @"..\packages\FSharp.Data.1.1.10\lib\net40\FSharp.Data.dll"

open FSharp.Data

(* 
Type Providers use schema information to generate types on-the-fly.
The obvious use case is to access data: here,
we use the CSV type provider from FSharp.Data 
to extract passenger information,
renaming properties for convenience:
- Pclass: Class, 
- Parch: ParentsOrChildren, 
- SibSp: SiblingsOrSpouse
We force inference to treat every feature as optional, 
assuming that any feature could have missing values.
*)

type Titanic = CsvProvider<"titanic.csv">
//                           Schema="PassengerId=int, Pclass->Class, Parch->ParentsOrChildren, SibSp->SiblingsOrSpouse", 
//                           SafeMode=true, 
//                           PreferOptionals=true>

let first = (new Titanic()).Data |> Seq.head

(* 
I can start accessing data from the World Bank,
in a statically typed manner, over the wire...
*)

let wb = WorldBankData.GetDataContext()
//wb.Countries.Haiti.Indicators.d
let largeCountries =
    query {
        for c in wb.Countries do
        where (c.Indicators.``Population, total``.[2000] > 100000000.)
        select (c.Name, c.CapitalCity) }
    |> Seq.iter (fun (name,city) -> printfn "%s (%s)" name city)

(* 
... or discover what R packages are installed,
and what functions are available to me...
*)

open RDotNet
open RProvider
open RProvider.``base``
open RProvider.graphics

let rng = System.Random()
let x = [ for i in 1 .. 100 -> rng.NextDouble() ]
x |> R.log |> R.plot

(*
... and combine all of this together.
*)

let china = wb.Countries.China

[ for y in 2000 .. 2010 -> china.Indicators.``GDP (current US$)``.[y]] |> R.barplot

let sample = 
    query {
        for c in wb.Countries do
        where (c.Indicators.``Population, total``.[2000] > 75000000.)
        select c }

let growth = [ for c in sample -> c.Indicators.``GDP growth (annual %)``.[2000] ]
let poll   = [ for c in sample -> c.Indicators.``CO2 emissions (metric tons per capita)``.[2000] ]
let youth  = [ for c in sample -> c.Indicators.``Population ages 0-14 (% of total)``.[2000] ]

[ "Growth", growth
  "Pollution", poll
  "Youth", youth ]
|> namedParams
|> R.data_frame
|> R.plot

// more elegant solution: use Deedle data frame...

// this example requires the rworldmap package to
// be installed on the local machine to run.

open RProvider.rworldmap

let popGrowth (country:WorldBankData.ServiceTypes.Country) =
    country.Indicators.``Population, total``.[2010]
    /country.Indicators.``Population, total``.[2000]

let dataframe =
    let codes, pops = 
        query { for country in wb.Countries -> 
                    country.Code, 
                    popGrowth country }
        |> Seq.toArray
        |> Array.unzip
    let data =
        [ "Code", codes |> box 
          "Pop", pops |> box ]
    R.data_frame (namedParams data)

let map = R.joinCountryData2Map(dataframe, "ISO3", "Code")
R.mapDevice()
R.mapCountryData(map,"Pop")