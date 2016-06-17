[<RequireQualifiedAccess>]
module Message

module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props
open Abstractions
 
type Model = {Date : System.DateTime; Text : string; Author : string}

type Component(cursor) = 
    inherit viewComponent<Model>(cursor)
    
    override x.render () = 
        let st = x.GetState ()
        let dt = st.Date
        let z= dt.ToShortTimeString()
        let message = sprintf "[%s] %s : %s" z st.Author st.Text
        
        R.div [P.ClassName "message" ]
            [ R.span [] (message |> unbox)]