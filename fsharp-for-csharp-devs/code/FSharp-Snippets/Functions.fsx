type Kitten = { Name: string; Age: int }

// List comprehensions: 
// let's just create a list of kittens 
let kittens = [
    { Name = "Alfred"; Age = 5 };
    { Name = "Biggie"; Age = 12 };
    { Name = "Cat"; Age = 2 }; ]

// this is not really needed here,
// but introduced for clarity
type Filter = Kitten -> bool

// List, Seq, ... modules provide
// functionality similar to LINQ
let giveMeKittens (kittens: Kitten list) (filter: Filter) =
    List.filter filter kittens

// List.

// You can declare a function like this;
// note we don't need parenthesis
let youngOne kitten = kitten.Age < 3

// the signature happens to match,
// so no fuss no muss you can just use it:
let youngUns = giveMeKittens kittens youngOne

// more interesting: a function with 2 arguments
// note that we still have no parenthesis
let youngerThan age kitten = kitten.Age < age

// Why would that be a good idea?
// First you can create a new function on the fly,
// by leaving some arguments "free", and
// "locking" some arguments (here we use a closure on 10):
let youngerThan10 = youngerThan 10

// this is handy in situations like this:
let stillYoung = giveMeKittens kittens (youngerThan 6)

// using pipe-forward, you can now replace f x y by y |> f x 
let oldSchool = youngerThan 6 { Name = "Albert"; Age = 11 }
let coolBeans = { Name = "Friday"; Age = 6 } |> youngerThan 5

// why is this awesomesauce?
// you can now build workflows;
// essentially you get fluent interfaces 
// everywhere, for free:

open System
open System.IO

let readLines path = File.ReadAllLines(path)
let saveAs path lines = File.WriteAllLines(path, lines)

let lowerCase (text: string) = text.ToLowerInvariant()
let split (text: string) = text.Split(' ')

let analyze source target =
    readLines source // we grab the text lines
    |> Array.map lowerCase
    |> Array.map split // LINQ style: we lowercase all
    |> Array.concat
    |> Seq.distinct
    |> Seq.sort
    |> Seq.toArray
    |> saveAs target

let desktop = Environment.SpecialFolder.Desktop
let root = Environment.GetFolderPath(desktop)
let source = root + @"\test.txt"
let target = root + @"\words.txt"

analyze source target