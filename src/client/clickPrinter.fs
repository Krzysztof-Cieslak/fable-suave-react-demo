[<RequireQualifiedAccess>]
module ClickPrinter

module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props
open Abstractions


type Component(cursor) = 
    inherit viewComponent<Counter.ClickCounter>(cursor)
    
    override x.render () = 
        let st = x.GetState ()
        
        R.span [] (sprintf "Number of clicks: %d. Last click %A" st.Value st.LastClick  |> unbox)