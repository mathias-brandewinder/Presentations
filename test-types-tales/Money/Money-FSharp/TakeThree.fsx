(*
Take three: using tuples and discriminated unions.
*)

// test case one: 2 * 5 USD = 10 USD

type Currency = 
    | USD
    | EUR

type Money = Currency * float

let (.*) multiplier (money:Money) = 
    let curr,value = money
    (curr, multiplier * value) 

let fiveDollars = (USD, 5.0)
let tenDollars = (USD, 10.0)

2.0 .* fiveDollars = tenDollars

// test case 2:
// if the rate is 1 EUR = 2 USD,
// then 10 EUR should convert to 20 USD

// this is what I need to write:
// requires rates, and a target currency
// convert rates USD (EUR, 10.0) = (USD, 20.0) 

type Rates = Currency -> Currency -> float

let convert (rates:Rates) (target:Currency) (money:Money) =
    let curr,amount = money
    let rate = rates curr target
    (target,amount*rate)

let rates origin destination =
    match (origin,destination) with
    | EUR,USD -> 2.0
    | _ -> failwith "undefined rate"

convert rates USD (EUR, 10.0) = (USD, 20.0) 

// whenever I see a possibly Exception, I think 
// "can I make that problem disappear with types?"

type SafeRates = Currency -> Currency -> float Option

// note how the type signature now nicely warns us
// that the conversion is not guaranteed...
// and we can't ignore missing cases.
let safeConvert (rates:SafeRates) (target:Currency) (money:Money) =
    let curr,amount = money
    let rate = rates curr target
    match rate with 
    | None -> None
    | Some(rate) ->
        Some(target,amount*rate)

// or, even nicer...
let safeConvert2 (rates:SafeRates) (target:Currency) (money:Money) =
    let curr,amount = money
    rates curr target
    |> Option.map (fun rate -> target,amount*rate)

let safeRates origin destination =
    match (origin,destination) with
    | EUR,USD -> Some(2.0)
    | _ -> None

safeConvert safeRates USD (EUR, 10.0) = Some(USD, 20.0) 
safeConvert safeRates EUR (USD, 10.0) = None

// we can make things even nicer, using |>

// inject today's rates
let todaysRates = safeConvert safeRates
// inject target currency
let toUSD = todaysRates USD

(EUR, 10.0) |> toUSD = Some(USD, 20.0)

// I can now convert a whole portfolio

let fullRates origin destination =
    match (origin,destination) with
    | EUR,EUR -> Some(1.0)
    | USD,USD -> Some(1.0)
    | EUR,USD -> Some(2.0)
    | USD,EUR -> Some(0.75)

let portfolioIn (rates:SafeRates) (target:Currency) (portfolio:Money list) =
    let converter = safeConvert rates target
    portfolio
    |> List.map converter
    |> List.choose id
    |> List.fold (fun (curr,total) (_,amount) -> 
        (curr,total+amount)) (target,0.)

[(EUR,10.); (USD,20.)] |> portfolioIn fullRates USD