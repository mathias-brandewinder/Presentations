// A quick sample of DiffSharp, an autodiff library:
// http://gbaydin.github.io/DiffSharp/

#r @"..\packages\DiffSharp.0.6.0\lib\DiffSharp.dll"
open DiffSharp.AD

let inline f x = sin (sqrt x)

// Derivative of f
let df = diff f

df (D 1.)

let inline foo x = x ** 2.
let dfoo = diff foo
foo (D 0.)
foo (D 2.)

// gradient descent...

let descent f x0 alpha epsilon =
    let rec search x =
        printfn "x = %.3f" (float x)
        let g = diff f x
        if abs g < epsilon
        then x
        else search (x - alpha * g)
    search x0

descent foo (D 10.) (D 0.1) (D 0.0001)

// A vector-to-scalar function
let g (x:_[]) = exp (x.[0] * x.[1]) + x.[2]

// Gradient of g
let gg = grad g 

gg ([| D 1.; D 2.; D 3.|])