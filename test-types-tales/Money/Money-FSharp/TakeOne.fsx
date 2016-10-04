(*
Take one: using records instead of classes.
I get value-wise equality out-of-the-box.
*)

type Dollars = 
    { USD:float }
    member this.MultiplyBy mult = 
        { this with 
            USD = this.USD * mult }
    static member (*) (mult,dollars) =
        { dollars with USD = dollars.USD * mult }
    
// this is equivalent to the unit test I would write,
// in a TDD style.
// I can start there, flesh out my design, and then
// simply copy-paste that code into implementation
// and tests once I am happy with the result.

let fiveDollars = { USD = 5.0 }
let tenDollars = { USD = 10.0 }
fiveDollars.MultiplyBy 2.0 = tenDollars

2.0 * fiveDollars = tenDollars

// I can make the API a bit nicer, by creating a 
// small function 'dollars'.

let dollars x = { USD = x }
2.0 * dollars 5.0 = dollars 10.0

// I can use UnQuote to make it more 'testy'

#r @"../packages/Unquote/lib/net45/Unquote.dll"
open Swensen.Unquote

test <@ 2.0 * fiveDollars = tenDollars @>

// ... and promote this to an actual test if I want to
