(*
Take one: using records instead of classes.
I get value-wise equality out-of-the-box.
*)

type Dollars = 
    { Amount:float }
    member this.MultiplyBy mult = 
        { this with 
            Amount = this.Amount * mult }
    static member (*) (mult,dollars) =
        { dollars with Amount = dollars.Amount * mult }
    
// this is equivalent to the unit test I would write,
// in a TDD style.
// I can start there, flesh out my design, and then
// simply copy-paste that code into implementation
// and tests once I am happy with the result.

let fiveDollars = { Amount = 5.0 }
fiveDollars.MultiplyBy 2.0 = { Amount = 10.0 }

2.0 * fiveDollars = { Amount = 10.0 }
