module App

open System
open Fable.Core
open Fable.Import
open Abstractions
open Abstractions.Operators
open Counter 

module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props  

type ViewModel = {
    Counter : CounterViewModel 
}
with static member Empty = {Counter = CounterViewModel.Empty}

let state = State.init ViewModel.Empty

let cursor = 
    ((fun x -> x.Counter), (fun t x -> {x with Counter = t}))
    |> Cursor.create state

let comp = createCounter cursor  


ReactDom.render( 
    comp.render (),
    Browser.document.getElementById "content"
)  
|> ignore