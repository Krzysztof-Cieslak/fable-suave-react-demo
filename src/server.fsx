#r "../packages/Suave/lib/net40/Suave.dll"
#load "model.fs"
    
open Suave
open Suave.Filters
open Suave.Successful
open Suave.Operators
open Suave.Files
open Suave.Sockets
open Suave.Sockets.Control
open Suave.WebSocket

let app =  
    GET >=> choose [ 
        path "/api" >=> OK "Hello World"
        path "/" >=> browseFileHome "index.html"
        browseHome 
    ]        
    