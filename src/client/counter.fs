[<RequireQualifiedAccess>]
module Counter

module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props
open Abstractions

type ClickCounter = {
    Value : int
    LastClick : System.DateTime
}
with static member Empty = {Value = 0; LastClick = System.DateTime.Now}

type ViewModel = {
    Count : int
    Click : ClickCounter
}
with static member Empty = {Count = 0; Click = ClickCounter.Empty}
     static member _Click = ((fun x -> x.Click), (fun t x -> {x with Click = t}))
     static member _Count = ((fun x -> x.Count), (fun t x -> {x with Count = t}))


type Component(cursor) = 
    inherit viewComponent<ViewModel>(cursor)
    
    override x.render () =  
        let st = x.GetState () 

        R.div [] [
            R.button [P.OnClick (fun e ->                 
                {st with Count = st.Count + 1; Click = { st.Click with Value = st.Click.Value + 1; LastClick = System.DateTime.Now}} |> x.Update ) ] [ unbox "Add" ]
            R.button [P.OnClick (fun e -> 
                 {st with Count = st.Count - 1; Click = { st.Click with Value = st.Click.Value + 1; LastClick = System.DateTime.Now}} |> x.Update ) ] [ unbox "Remove" ]
            R.span [] [unbox st.Count ]
        ] 
        
 