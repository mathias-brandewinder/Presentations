namespace Initrode

module Codebase =

    type Dollars = 
        { USD:float }
        member this.MultiplyBy mult = 
            { this with 
                USD = this.USD * mult }
        static member (*) (mult,dollars) =
            { dollars with USD = dollars.USD * mult }

    let dollars x = { USD = x }


module Tests =

    open NUnit.Framework
    open Swensen.Unquote
    open Codebase

    printfn "this is where our tests go."

    [<Test>]
    // no need for under_score or camelCase...
    // reads like English.
    let ``2 x 5 USD = 10 USD`` () =

        test <@ 2.0 * dollars 5.0 = dollars 10.0 @> 
