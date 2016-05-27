[<RequireQualifiedAccess>]
module Counter

module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props
open Abstractions

type ViewModel = {
    Count : int
}
with static member Empty = {Count = 0}

type counterComponent(cursor) = 
    inherit viewComponent<ViewModel>(cursor)
    
    member x.render () =  
        let st = x.GetState () 

        R.div [] [
            R.button [P.OnClick (fun e -> {st with Count = st.Count + 1} |> x.Update ) ] [ unbox "Add" ]
            R.button [P.OnClick (fun e -> {st with Count = st.Count - 1} |> x.Update ) ] [ unbox "Remove" ]
            R.span [] [unbox st.Count ]
        ]
         
        
        
let create cursor = 
    new counterComponent(cursor)
    //|> R.toPlainJsObj 
    //|> unbox
    |> createComponent<ViewModel>
        
 