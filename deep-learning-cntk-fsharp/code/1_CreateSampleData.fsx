(*
Creating a slightly noisy dataset of 1s and 2s
*)

open System
open System.IO

let rng = System.Random ()

let noise (c:char) =
    if rng.NextDouble () > 0.01
    then c
    else 
        if c = '0' then '1'
        elif c = '1' then '0'
        else c

let one = 
    [
        "0 0 0 0 0 0 0 0"  
        "0 0 0 0 1 0 0 0"  
        "0 0 0 1 1 0 0 0"  
        "0 0 1 0 1 0 0 0"  
        "0 0 0 0 1 0 0 0"  
        "0 0 0 0 1 0 0 0"  
        "0 0 1 1 1 1 1 0"  
        "0 0 0 0 0 0 0 0"         
    ]

let two = 
    [
        "0 0 0 0 0 0 0 0"  
        "0 0 1 1 1 1 0 0"  
        "0 1 0 0 0 0 1 0"  
        "0 0 0 0 0 0 1 0"  
        "0 0 1 1 1 1 0 0"  
        "0 1 0 0 0 0 0 0"  
        "0 1 1 1 1 1 1 0"  
        "0 0 0 0 0 0 0 0" 
    ]

let noisify bitmap = 
    bitmap
    |> String.concat "  "
    |> Seq.map (fun c -> noise c |> string)
    |> String.concat ""

let createData () = 
    [
        for _ in 1 .. 32 do
            yield 
                sprintf "|labels 1 |pixels %s" (noisify one)
        for _ in 1 .. 32 do
            yield 
                sprintf "|labels 0 |pixels %s" (noisify two)       
    ]
    |> fun data -> File.WriteAllLines(__SOURCE_DIRECTORY__ + "/images", data)