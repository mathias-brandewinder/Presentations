let blacklist = [
    "System.IO"
    "System.Threading" ]

let (|Bad|_|) (code:string) =
    blacklist
    |> Seq.tryFind (fun threat -> code.Contains(threat))

let analyze (code:string) =
    match code with
    | Bad(threat) -> printfn "Warning: %s" threat
    | _ -> printfn "All clear"

analyze "@fsibot is awesome"
analyze "System.IO.FormatAllDrives()"

// wait... this is also valid code!
// in C# too, BTW.

System.Console.WriteLine("Hello @fsibot")
System. Console.           WriteLine("Hello @fsibot")

let (|Badder|_|) (code:string) =
    blacklist
    |> Seq.tryFind (fun threat -> code.Replace(" ","").Contains(threat))

let analyzeMore (code:string) =
    match code with
    | Badder(threat) -> printfn "Warning: %s" threat
    | _ -> printfn "All clear"

analyzeMore "System. IO.  DoBadThings(now)"

// but there is more...

System. Console. (*Hello this is an inline comment*) WriteLine("Oooops")

// this is obviously harder to handle.
// we could try to Regex the hell out of this, 
// but this isn't very pleasant.

// the compiler shouldn't have that problem.

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


let (parsed, _, _) = fsiSession.ParseAndCheckInteraction """System.Console.WriteLine("hello") """
parsed.ParseTree.Value

let ast code =
    let (parsed, _, _) = fsiSession.ParseAndCheckInteraction code
    parsed.ParseTree.Value
    
ast "System. (*some nasty comments*) Random()"