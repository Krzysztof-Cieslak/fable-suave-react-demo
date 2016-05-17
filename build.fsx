// --------------------------------------------------------------------------------------
// A simple FAKE build script that:
//  1) Hosts Suave server locally & reloads web part that is defined in 'server.fsx'
//  2) Deploys the web application to Azure web sites when called with 'build deploy'
// --------------------------------------------------------------------------------------

#r "packages/FSharp.Compiler.Service/lib/net45/FSharp.Compiler.Service.dll"
#r "packages/Suave/lib/net40/Suave.dll"
#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.NpmHelper

open System
open System.IO
open Suave
open Suave.Web
open Microsoft.FSharp.Compiler.Interactive.Shell

let sbOut = new Text.StringBuilder()
let sbErr = new Text.StringBuilder()

let fsiSession =
  let inStream = new StringReader("")
  let outStream = new StringWriter(sbOut)
  let errStream = new StringWriter(sbErr)
  let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
  let argv = Array.append [|"/fake/fsi.exe"; "--quiet"; "--noninteractive"; "-d:DO_NOT_START_SERVER"|] [||]
  FsiEvaluationSession.Create(fsiConfig, argv, inStream, outStream, errStream)

let reportFsiError (e:exn) =
  traceError "Reloading server.fsx script failed."
  traceError (sprintf "Message: %s\nError: %s" e.Message (sbErr.ToString().Trim()))
  sbErr.Clear() |> ignore

let reloadScript () =
  try
    traceImportant "Reloading server.fsx script..."
    let appFsx = __SOURCE_DIRECTORY__ @@ "src" @@ "server.fsx"
    fsiSession.EvalInteraction(sprintf "#load @\"%s\"" appFsx)
    fsiSession.EvalInteraction("open Server")
    match fsiSession.EvalExpression("app") with
    | Some app -> Some(app.ReflectionValue :?> WebPart)
    | None -> failwith "Couldn't get 'app' value"
  with e -> reportFsiError e; None

// --------------------------------------------------------------------------------------
// Suave server that redirects all request to currently loaded version
// --------------------------------------------------------------------------------------

let currentApp = ref (fun _ -> async { return None })

let serverConfig =
  { defaultConfig with
      homeFolder = Some (__SOURCE_DIRECTORY__ @@ "public")
      logger = Logging.Loggers.saneDefaultsFor Logging.LogLevel.Debug
      bindings = [ HttpBinding.mkSimple HTTP  "127.0.0.1" 8034] }

let reloadAppServer () =
  reloadScript() |> Option.iter (fun app ->
    currentApp.Value <- app
    traceImportant "New version of server.fsx loaded!" )
    
let startNpmWatcher () = 
  async {
    return Npm (fun p -> { p with Command = Run "watch" } )
  } |> Async.Start

Target "develop" (fun _ ->
  let app ctx = currentApp.Value ctx
  let _, server = startWebServerAsync serverConfig app
  
  reloadAppServer()
  Async.Start(server)
  
  startNpmWatcher ()
  use watcher = !! (__SOURCE_DIRECTORY__ @@ "src" @@ "*.*") |> WatchChanges (fun _ -> reloadAppServer())
  
  System.Diagnostics.Process.Start("http://localhost:8034/index.html") |> ignore
  System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite)
)

RunTargetOrDefault "develop" 