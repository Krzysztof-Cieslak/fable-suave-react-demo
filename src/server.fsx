#r "../packages/Suave/lib/net40/Suave.dll"
    
open Suave
open Suave.Filters
open Suave.Successful
open Suave.Operators
open Suave.Files

let app =  
    GET >=> choose [ 
        path "/api" >=> OK "Hello World"
        path "/" >=> file "index.html"
        browseHome 
    ]        
    