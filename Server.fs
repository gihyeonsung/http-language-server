module HttpLanguageServer.Server

open System
open System.Threading
open System.Threading.Tasks

module Json =
  open System.Text.Json

  let options =
    JsonSerializerOptions JsonSerializerDefaults.Web
    |> fun o -> o.WriteIndented <- true; o

  let sz a = JsonSerializer.Serialize (a, options)

  let ds (s: string) = JsonSerializer.Deserialize (s, options)

module Hover =
  open FSharp.Data

  let httpClient = new System.Net.Http.HttpClient ()

  type Request = {
    TextDocument: string
    Position: {|
      Line: uint
      Character: uint
    |}
  }

  type Response = {
    Value: string
  }

  [<Literal>]
  let BaseurlMethod = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/"

  [<Literal>]
  let BaseurlHeaderName = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/"

  [<Literal>]
  let CssSelectorMain = "#content"

  let fetchHttpMethodInfo s =
    async {
      let! html = HtmlDocument.AsyncLoad (BaseurlMethod + s)
      let content =
        html
        |> fun a -> a.CssSelect CssSelectorMain
        |> List.tryExactlyOne
        |> Option.map HtmlNode.innerText
      return
        match content with
        | Some s -> Ok s
        | None -> Error "no contents"
    }

  let fetchHttpHeaderNameInfo s =
    async {
      let! html = HtmlDocument.AsyncLoad (BaseurlHeaderName + s)
      let content =
        html
        |> fun a -> a.CssSelect CssSelectorMain
        |> List.tryExactlyOne
        |> Option.map HtmlNode.innerText
      return
        match content with
        | Some s -> Ok s
        | None -> Error "no contents"
    }

  let handle (req: Request): Async<Response> =
    async {
      return
        match Parser.parse req.TextDocument with
        | Ok req -> { Value = "" }
        | Error err -> { Value = "" }
    }
