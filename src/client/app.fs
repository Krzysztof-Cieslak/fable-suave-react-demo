module App

open System
open Fable.Core
open Fable.Import
open Abstractions 
open Abstractions.Operators

module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props  

type ViewModel = {
    MessageList : MessageList.Model 
}
with static member Empty = {MessageList = MessageList.Model.Empty} 
     static member _MessageList =  ((fun x -> x.MessageList), (fun t x -> {x with MessageList = t}))

let state = ref <| State.init ViewModel.Empty
   
let msgListCurosr = 
    ViewModel._MessageList
    |> Cursor.create state

ReactDom.render(  
    R.div [] [
        R.com<MessageList.Component,_,_> msgListCurosr []
    ],
    Browser.document.getElementById "content"
)  
|> ignore