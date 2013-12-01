#r @"..\packages\MathNet.Numerics.2.6.2\lib\net40\MathNet.Numerics.dll"
#r @"..\packages\MathNet.Numerics.FSharp.2.6.0\lib\net40\MathNet.Numerics.FSharp.dll"

open MathNet.Numerics.LinearAlgebra.Generic
open MathNet.Numerics.LinearAlgebra.Double

(*
List and Array comprehension syntax in F#
result in very natural-looking algebra code,
which will look familar to anyone who has 
been using MatLab or Octave.
*)

let A = matrix [ [ 1.; 2.; 3.; ];
                 [ 4.; 5.; 6.; ];
                 [ 7.; 8.; 9.; ] ]

let B = vector [ 1.; 2.; 3. ]

let C = B * A

(* 
Vector & Matrix slicing are supported, too:
*)

let D = B.[1..2] * A.[1..2, 0..1]

(*
Algebra is an essential component of Machine Learning;
the first reason is that a large proportion of problems
can be expressed in a very compact form using Algebra.
For instance, linear regression can be stated as 
Beta = (X.T * X)^-1 * X.T * y
*)

let features = 500
let observations = 5000
let rng = System.Random()

let trueBeta = vector [ for f in 1 .. features -> rng.NextDouble() ]

let X = matrix [ for obs in 1 .. observations -> 
                   [ for f in 1 .. features -> rng.NextDouble() ] ]

let Y = vector [ for obs in 1 .. observations -> trueBeta * X.Row(obs - 1) ]

// FSI has a built-in timer which comes in handy for quick-tuning
#time

let beta = (X.Transpose() * X).Inverse() * X.Transpose() * Y

let error = (trueBeta - beta) |> Seq.map abs |> Seq.max |> printfn "%.5f"

(*
The second reason Algebra matter is that it's also heavily used
for graphics, and thanks to gamers, there is built-in hardware
support for problems expressed in algebra terms.
Math.NET supports for instance MKL:
*)

System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

open MathNet.Numerics
open MathNet.Numerics.Algorithms.LinearAlgebra.Mkl
Control.LinearAlgebraProvider <- MklLinearAlgebraProvider()

let fastBeta = (X.Transpose() * X).Inverse() * X.Transpose() * Y

(*
This part is not specific to F#; it's available to C# as well.
However, one interesting aspect of F# is its roots in ML,
aka MetaLanguage, which was developed with compilers in mind.
As a side-effect, F# has interesting things to offer, like
compiler to CUDA / GPU with Alea.CuBase (also to JavaScript,
but it seems everyone is doing that these days...).
*)