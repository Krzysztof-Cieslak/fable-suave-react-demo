module Counter

module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props
open Abstractions

type CounterViewModel = {
    Count : int
}
with static member Empty = {Count = 0}

let createCounter (cursor : cursor<CounterViewModel>) = 
    let render (update : CounterViewModel -> unit) (st : CounterViewModel) = 
        R.div [] [
            R.button [P.OnClick (fun e -> {st with Count = st.Count + 1} |> update ) ] [ unbox "Add" ]
            R.button [P.OnClick (fun e -> {st with Count = st.Count - 1} |> update ) ] [ unbox "Remove" ]
            R.span [] [unbox st.Count ]
        ]
        
    ViewComponent.create cursor render 
 