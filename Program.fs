module HttpLanguageServer.Program

open System
open System.IO
open System.IO.Pipes
open System.Threading
open System.Threading.Tasks
open AustinHarris.JsonRpc
open System

type HoverService () =
  inherit JsonRpcService ()

  [<JsonRpcMethod("textDocument/hover")>]
  member _.TextDocumentHover (req: Server.Hover.Request): Task<Server.Hover.Response> =
    Async.StartAsTask (Server.Hover.handle req)

type Server () =


[<EntryPoint>]
let main args =
  let stdin = Console.OpenStandardInput ()
  let stdout = Console.OpenStandardOutput ()
  let router = Server.Router ()
  use server = StreamJsonRpc.JsonRpc.Attach (stdout, stdin, router)
  let rec loop () =
    async {
      server.StartListening ()
      do! server.Completion |> Async.AwaitTask
      do! loop ()
    }
  loop () |> Async.RunSynchronously
  0
