module Abstractions

open System
open Fable.Core
open Fable.Import 
module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props


type state<'a> = private {
    mutable History : List<'a> 
    _stateUpdated : Event<'a>
} 
with member this.StateUpdate = this._stateUpdated.Publish

module State = 
    let init value = 
        {History = [value]; _stateUpdated = Event<_>()}
        
    let current state =
        state.History |> List.head
        
    let update state newValue= 
        state.History <- (newValue :: state.History)
        state._stateUpdated.Trigger newValue

type lens<'a,'b> = ('a -> 'b) * ('b -> 'a -> 'a)

module Lens = 
    let get ((g,_) : lens<'a,'b>) target =
        g target 
    
    let set ((_,s) : lens<'a,'b>) target =
        s target     

    let combine ((g2, s2): lens<'b,'c>) ((g1, s1): lens<'a,'b>) =
        (fun a -> g2 (g1 a)), (fun c a -> s1 (s2 c (g1 a)) a) : lens<'a,'c>
        
module Operators = 
    let (>->) a b = Lens.combine b a
    

type cursor<'a> = (unit -> 'a) * ('a -> unit) * IEvent<'a> 

module Cursor = 
    let create state lens : cursor<_> = 
        let getter () = 
            state 
            |> State.current 
            |> Lens.get lens
    
        let setter value =
            state 
            |> State.current 
            |> Lens.set lens value
            |> State.update state
        
        let stream = 
            state.StateUpdate
            |> Event.map (Lens.get lens)
            
        (getter, setter, stream)

     
[<AbstractClass>]
type viewComponent<'a> (cursor : cursor<'a>) = 
    inherit R.Component<cursor<'a>, 'a> (cursor)
    
    let (getState, update, stream) = cursor
    
    member x.getInitialState () = 
        getState () |> x.setState
        
    member x.componentWillMount () = 
        stream |> Observable.add x.setState
        
    member x.Update = update
    
    member x.GetState = getState
    
    abstract member Render : unit -> React.ReactElement<obj>
    
    
module ViewComponent = 
    let create cursor render = 
        {
            new viewComponent<_>(cursor) with
                member this.Render () = 
                    let v = this.GetState ()                  
                    render this.Update v
        }