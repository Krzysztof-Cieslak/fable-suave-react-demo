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
     static member _Counter =  ((fun x -> x.Counter), (fun t x -> {x with Counter = t}))

let state = ref <| State.init ViewModel.Empty
   
let counterCursor = 
    ViewModel._Counter
    |> Cursor.create state

let clickPrinterCursor = 
    ViewModel._Counter >-> Counter.ViewModel._Click
    |> Cursor.create state

ReactDom.render(  
    R.div [] [
        R.com<Counter.Component,_,_> counterCursor []
        R.com<ClickPrinter.Component,_,_> clickPrinterCursor []
    ],
    Browser.document.getElementById "content"
)  
|> ignore