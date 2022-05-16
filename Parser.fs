module HttpLanguageServer.Parser

open Syntax

open FParsec
open FParsec.CharParsers
open FParsec.Primitives

let pMethod: Parser<Method, unit> =
  let pGet = stringReturn "GET" Get
  let pHead = stringReturn "HEAD" Get
  let pUnknown = many1Satisfy (fun c -> c <> ' ') |>> Unknown
  pGet <|> pHead <|> pUnknown <?> "method"

let pUri: Parser<string, unit> = many1Satisfy (fun c -> c <> ' ')

let pVersion: Parser<string, unit> = manyCharsTill anyChar newline

let pHeader: Parser<Header, unit> =
  pipe2
    ((many1CharsTill anyChar (pchar ':')) .>> spaces)
    (manyCharsTill anyChar newline)
    (fun n v -> { Name = n; Value = v })

let pHeaders: Parser<Set<Header>, unit> = many pHeader |>> Set.ofList

let pRequest: Parser<Request, unit> =
  pipe4
    (pMethod .>> spaces1)
    (pUri .>> spaces1)
    pVersion
    (pHeaders .>> eof)
    (fun m u v h -> { Method = m; Uri = u; Version = v; Headers = h})

let parse (text: string): Result<Request, string> =
  match run pRequest text with
  | Success (a, _, _) -> Result.Ok a
  | Failure (err, _, _) -> Result.Error err
