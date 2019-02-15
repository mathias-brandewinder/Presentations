namespace Demo

module Domain = 

    type Position = { 
        North: int 
        West: int 
        }

    type Direction = 
        | N
        | W
        | S
        | E

    type Action = 
        | Move of Direction

    let move pos dir = 
        match dir with 
        | N -> { pos with North = pos.North + 1 }
        | S -> { pos with North = pos.North - 1 }
        | W -> { pos with West = pos.West + 1 }
        | E -> { pos with West = pos.West - 1 }

    let update action pos = 
        match action with
        | Move dir -> move pos dir

    let initial = { North = 0; West = 0 }

    // update (Move N) initial

    // initial
    // |> update (Move N)
    // |> update (Move W)

    // (initial, [ Move N; Move N; Move W ]) 
    // ||> List.fold (fun state action -> update action state)
