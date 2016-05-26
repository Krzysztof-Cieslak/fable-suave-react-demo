module Model 

type Commands =
    | Join of Username : string
    | SendMessage of Message : string 
    | RemoveMessage of Id : string

type Event = 
    | UserJoined of Username : string
    | MessageSent of Id : string * Message : string * Date : System.DateTime
    | MessageRemoved of Id : string