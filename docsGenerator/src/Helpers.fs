[<AutoOpen>]
module Global

open Fable.Helpers.React
open Fable.Helpers.React.Props

let whitespace =
    span [ DangerouslySetInnerHTML { __html = " " } ]
        [ ]

module Helpers =

    open System.Collections.Generic
    open Fable.Core.JsInterop
    open Fable.Import
    open Fable.Import.Node
    open Fable.Import.Node.Globals
    open Fable.PowerPack

    let makeHtml (_:string) :  JS.Promise<string> = importMember "./js/utils.js"

    /// Resolves a path to prevent using location of target JS file
    /// Note the function is inline so `__dirname` will belong to the calling file
    let inline resolve (path: string) =
        Exports.path.resolve(__dirname, path)

    /// Parses a markdown file
    let parseMarkdown (path: string) =
        Exports.fs.readFileSync(path).toString() |> makeHtml

    /// Parses a React element invoking ReactDOMServer.renderToString
    let parseReact (el: React.ReactElement) =
        ReactDomServer.renderToString el

    /// Parses a React element invoking ReactDOMServer.renderToStaticMarkup
    let parseReactStatic (el: React.ReactElement) =
        ReactDomServer.renderToStaticMarkup el

    let rec private ensureDirExists (dir: string) (cont: (unit->unit) option) =
        if Exports.fs.existsSync !^dir then
            match cont with Some c -> c() | None -> ()
        else
            ensureDirExists (Exports.path.dirname dir) (Some (fun () ->
                if not(Exports.fs.existsSync !^dir) then
                    Exports.fs?mkdirSync(dir) |> ignore
                match cont with Some c -> c() | None -> ()
            ))

    let writeFile (path: string) (content: string) =
        ensureDirExists (Exports.path.dirname path) None
        Exports.fs.writeFileSync(path, content)

    let readFile (path: string) =
        Exports.fs.readFileSync(path).toString()

    open Fable.Core
    open Fable.Helpers.React
    open Fable.Helpers.React.Props
    open Fulma

    type DangerousInnerHtml =
        { __html : string }

    let htmlFromMarkdown str =
        promise {
            let! html = makeHtml str
            return div [ DangerouslySetInnerHTML { __html = html } ] [ ]
        }

    let contentFromMarkdown str =
        promise {
            let! html = makeHtml str
            return Content.content [ Content.Props [ DangerouslySetInnerHTML { __html = html } ] ]
                [ ]
        }
