// Goal: model dice rolls as expressions
// along the lines of damage = 6d10 + 4 + 2d6

type Dice = 
    | D of int 
    static member (*) (times: int, dice) =
        Roll(times, dice)
and DiceRoll = 
    | Roll of int * Dice 
    | Value of int
    | Add of list<DiceRoll>
    static member (+) (roll1: DiceRoll, roll2: DiceRoll) = 
        Add [ roll1; roll2 ]
    static member (+) (roll: DiceRoll, value: int) = 
        Add [ roll; Value value ]
    static member (+) (value: int, roll: DiceRoll) = 
        Add [ Value value; roll ]

Add [ Roll (6, D 10); Value 4; Roll (2, D 6) ]

let d4 = D 4
let d6 = D 6
let d8 = D 8
let d10 = D 10
let d12 = D 12
let d20 = D 20

Add [ Roll (6, d10); Value 4; Roll (2, d6) ]

Roll(2, d10) + 10

10 + Roll(2, d10)

2 * (D 10)
2 * d10
10 + 2 * d10 + 5 + 4 * d8

let rng = System.Random ()

let rec eval (roll: DiceRoll) = 
    match roll with
    | Value x -> x
    | Roll(times, D sides) -> 
        Seq.init times (fun _ -> rng.Next(1, sides + 1))
        |> Seq.sum   
    | Add rolls ->
        rolls |> Seq.sumBy eval

eval (Add [ Roll (4, D 10); Roll (2, D 12); Value 2 ])

4 * d10 + 2 * d12 + 2 |> eval 
