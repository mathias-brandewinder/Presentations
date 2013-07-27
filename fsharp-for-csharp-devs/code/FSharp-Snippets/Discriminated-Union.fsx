// a discriminated union defines "cases"
type Boolean = 
    | True 
    | False

// the "mystery default" is gone

// also note what happens if you add
// a "Maybe" case to Boolean
let truth (b: Boolean) =
    match b with
    | True -> "Yeah it's true"
    | False -> "Nope, not right"


// but wait! There's more!
type Result =
    | Value of float
    | Error of string

// you can now send a clear signal
// to the consumer of your code
let division (x: float) (y: float) =
    if y = 0.0
    then Error("Nope, can't do")
    else Value(x / y)

// ... which comes in handy if the result
// could be "nothing". Bye bye, nulls!
let maxFromList (data: int list) =
    match data with
    | [] -> None
    | x  -> Some(List.max x)

// now you have a signature that is explicit:
// sometimes, that operation won't be able
// to give you "something"

let printMax (data: int list) =
    let max = maxFromList data
    match max with
    | None -> "Dude there is no max here"
    | Some(value) -> sprintf "Maximum is %i" value