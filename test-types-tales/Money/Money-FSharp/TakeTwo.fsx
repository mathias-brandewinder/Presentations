(*
Take two: using units of measure.
One-liner :)
*)

[<Measure>] type Dollars

5.0<Dollars> * 2.0 = 10.0<Dollars> 

// a bit overkill? Is this really useful?
let squareDollars = 10.0<Dollars> * 20.0<Dollars>

// I could define a salary
[<Measure>] type Months
100000.0<Dollars> / 12.0<Months>