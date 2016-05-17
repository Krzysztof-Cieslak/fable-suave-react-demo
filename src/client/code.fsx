#r "../../node_modules/fable-core/Fable.Core.dll"
#load "../../node_modules/fable-import-react/Fable.Import.React.fs" 
#load "../../node_modules/fable-import-react/Fable.Helpers.React.fs"

open System
open Fable.Core
open Fable.Import
module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props

ReactDom.render(
    R.span [] [unbox "test"],
    Browser.document.getElementById "content"
)  