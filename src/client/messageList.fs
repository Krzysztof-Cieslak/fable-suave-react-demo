[<RequireQualifiedAccess>]
module MessageList

module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props
open Abstractions
 
type Model = {Messages : Message.Model list}
with static member Empty = {Messages = []}

type Component(cursor) = 
    inherit viewComponent<Model>(cursor)
    
    override x.render () = 
        let st = x.GetState ()
        
        let messages = st.Messages |> List.mapi (fun i m ->
            let l : lens<Model, Message.Model> = (fun a -> a.Messages |> List.item i), (fun a b -> ())
            let crs = Cursor.combine cursor l
            R.com<Message.Component,_,_> crs []
        )
        
        R.div [P.ClassName "messageList" ] messages
           