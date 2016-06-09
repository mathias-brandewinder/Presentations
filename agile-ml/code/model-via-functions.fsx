// Object oriented approach

type Observation = {
    Search: string
    Product: string }
    with member this.SearchLength =
        this.Search.Length

// Properties as functions

let searchLength (obs:Observation) =
    obs.Search.Length

// "object" as a bag of functions
let model = [
    fun obs -> searchLength obs
    ]
