(*
Initializing a FSI Session

See the following link for more information:
https://fsharp.github.io/FSharp.Compiler.Service/interactive.html
*)

#I @"../packages/"
#r @"FSharp.Compiler.Service.0.0.76/lib/net45/FSharp.Compiler.Service.dll"
open Microsoft.FSharp.Compiler.Interactive.Shell

open System
open System.IO
open System.Text

// Intialize output and input streams
let sbOut = new StringBuilder()
let sbErr = new StringBuilder()
let inStream = new StringReader("")
let outStream = new StringWriter(sbOut)
let errStream = new StringWriter(sbErr)

// Build command line arguments & start FSI session
let argv = [| "C:\\fsi.exe" |]
let allArgs = Array.append argv [|"--noninteractive"|]

let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
let fsiSession = FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, outStream, errStream)

(*
We can now use the FSI session
*)

// fsiSession.

fsiSession.EvalExpression "1 + 2"