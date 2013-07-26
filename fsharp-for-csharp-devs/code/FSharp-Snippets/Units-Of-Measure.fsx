[<Measure>] type miles
[<Measure>] type celcius

let distance1 = 10.0<miles>
let distance2 = 20.0<miles>

// that works just fine,
// and distance is a float<miles>
let distance = distance1 + distance2

let temperature1 = 30.0<celcius>

// won't work, thank you F#!
//let ooops = temperature1 + distance1

// now this is getting funky
// check the unit on that bad boy...
let goCrazy = distance1 * distance2 / temperature1

let evenCrazier = 50.0<miles^2/celcius>
let myOhMy = goCrazy + evenCrazier

let okThatsCool = myOhMy / distance1