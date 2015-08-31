type Currency = 
    | USD
    | EUR

type Money = Currency * float

let (.*) multiplier (money:Money) = 
    let curr,value = money
    (curr, multiplier * value) 

// what are invariants of the system?
// 0 * (USD, x) should be (USD, 0)

let ``Zero times any Money should be Zero`` (amount:float) =
    0.0 .* (USD,amount) = (USD,0.0)

#r @"../packages/FsCheck.2.0.7/lib/net45/FsCheck.dll"
open FsCheck

// generate a bunch of cases,
// and throw it at our invariant.
// does it break?
Check.Quick ``Zero times any Money should be Zero``

// same thing, but showing each check
Check.Verbose ``Zero times any Money should be Zero``

// more creative use: can we find a way 
// to make free money, that is, can we
// identify an arbitrage?

type Rates = Currency -> Currency -> float

let convert (rates:Rates) (target:Currency) (money:Money) =
    let curr,amount = money
    let rate = rates curr target
    (target,amount*rate)

let rates origin destination =
    match (origin,destination) with
    | EUR,EUR -> 1.0
    | USD,USD -> 1.0
    | EUR,USD -> 2.0
    | USD,EUR -> 0.6

let ``Converting back and forth should never make free money`` (origin:Currency) (target:Currency) =
    let exchange = convert rates 
    let (_,finalAmount) = (origin,1.0) |> exchange target |> exchange origin 
    finalAmount <= 1.0

Check.Verbose ``Converting back and forth should never make free money``

(EUR,1.0) |> convert rates USD |> convert rates EUR 