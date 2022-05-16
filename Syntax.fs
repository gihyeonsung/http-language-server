module HttpLanguageServer.Syntax

type Position = {
  Line: uint
  Column: uint
}

type Range = {
  File: string
  Start: Position
  End: Position
}

type Method =
  | Get
  | Head
  | Unknown of string
with
  override this.ToString () =
    match this with
    | Get -> "GET"
    | Head -> "HEAD"
    | Unknown s -> s

type Header = {
  Name: string
  Value: string
}
with
  override this.ToString () = $"{this.Name}: {this.Value}"

type Request = {
  Method: Method
  Uri: string
  Version: string
  Headers: Set<Header>
}
with
  override this.ToString () =
    let startLine = $"{this.Method} {this.Uri} {this.Version}\n"
    let headers = Set.map string this.Headers |> String.concat "\n"
    startLine + headers

