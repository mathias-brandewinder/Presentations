type Kitten = { Name: string; Age: int }

// List comprehensions: 
// let's just create a list of kittens 
let kittens = [
    { Name = "Alfred"; Age = 5 };
    { Name = "Biggie"; Age = 12 };
    { Name = "Cat"; Age = 2 }; ]

type Filter = Kitten -> bool

let giveMeKittens (kittens: Kitten list) filter =
    List.filter filter kittens

let youngOne kitten = kitten.Age < 3

let youngUns = giveMeKittens kittens youngOne

let youngerThan age kitten = kitten.Age < age

let stillYoung = giveMeKittens kittens (youngerThan 6)

