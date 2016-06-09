namespace HomeDepot

open HomeDepot.Core

module Features =

    let ``search length`` : Feature =
        fun obs -> obs.Search.Length |> float
    
    let ``product title length`` : Feature = 
        fun obs -> obs.Product.Length |> float

    let ``matching words`` : Feature =
        fun obs ->
            let search = obs.Search.Split ' ' |> set
            let product = obs.Product.Split ' ' |> set
            Set.intersect search product |> Set.count |> float
