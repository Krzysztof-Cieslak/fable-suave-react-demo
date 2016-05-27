module App

open System
open Fable.Core
open Fable.Import
open Abstractions
open Abstractions.Operators

module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props  

type ViewModel = {
    Counter : Counter.ViewModel 
}
with static member Empty = {Counter = Counter.ViewModel.Empty}

let state = ref <| State.init ViewModel.Empty
   
let counter  = 
    ((fun x -> x.Counter), (fun t x -> {x with Counter = t}))
    |> Cursor.create state
    |> Counter.create  

ReactDom.render(  
    counter,
    Browser.document.getElementById "content"
)  
|> ignore