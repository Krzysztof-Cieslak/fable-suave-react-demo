#r "../../node_modules/fable-core/Fable.Core.dll"
#load "../../node_modules/fable-import-react/Fable.Import.React.fs" 
#load "../../node_modules/fable-import-react/Fable.Helpers.React.fs"

open System
open Fable.Core
open Fable.Import
module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props

type State<'a> = private {
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

type Lens<'a,'b> = ('a -> 'b) * ('b -> 'a -> 'a)

module Lens = 
    let get ((g,_) : Lens<'a,'b>) target =
        g target 
    
    let set ((_,s) : Lens<'a,'b>) target =
        s target     

    let combine ((g2, s2): Lens<'b,'c>) ((g1, s1): Lens<'a,'b>) =
        (fun a -> g2 (g1 a)), (fun c a -> s1 (s2 c (g1 a)) a) : Lens<'a,'c>
        
module Operators = 
    let (>->) a b = Lens.combine b a
    

type Cursor<'a> = (unit -> 'a) * ('a -> unit) * IEvent<'a> 

module Cursor = 
    let create state lens : Cursor<_> = 
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
type ViewComponent<'a> (cursor : Cursor<'a>) = 
    inherit R.Component<Cursor<'a>, 'a> (cursor)
    
    let (getState, update, stream) = cursor
    
    member x.getInitialState () = 
        getState () |> x.setState
        
    member x.componentWillMount () = 
        stream |> Event.add x.setState
        
    member x.Update = update
        
    member x.ModelStream = stream
    
    abstract member Render : unit -> React.ReactElement<obj>
    
    
module ViewComponent = 
    let create cursor render = 
        {
            new ViewComponent<_>(cursor) with
                member this.Render () = 
                    render this.Update this.ModelStream
        }