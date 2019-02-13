module Main

open Fable.Import

open Fable.Core
open Fable.Core.JsInterop
open Fulma
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Thoth.Json

open System.Text.RegularExpressions


let pageTemplate (pageData : PageData) =
    let tocContent =
        let result = tocRegex.Match(pageData.PageContent)

        if result.Success then
            result.Value
        else
            """<nav class="toc-container"><\/nav>"""

    let pageContent =
        tocRegex.Replace(pageData.PageContent, "")


    |> Helpers.parseReactStatic
    |> Helpers.writeFile output

let isNotNull (o : 'T) =
    not (isNull o)

let readFile path =
    Promise.create (fun resole reject ->
        Node.Exports.fs.readFile(path, (fun err buffer ->
            match err with
            | Some err -> err :?> System.Exception |> reject
            | None -> buffer.toString() |> resole
        ))
    )

[<Emit("require($0)")>]
let require<'T> (modulePath : string) : 'T = jsNative

let processMarkdown (path : string) =
    promise {
        let! fileContent = readFile path
        let fm = FrontMatter.fm.Invoke(fileContent)

        match Decode.fromValue "$" PageContext.Decoder fm.attributes with
        | Error msg ->
            printfn "Seems like your the front matter attributes of `%s` are invalid" path
            printfn "%s" msg
        | Ok context ->
            match context.PostRenderDemos with
            | Some config ->
                let directory = Node.Exports.path.dirname(path)
                let scriptPath = Node.Exports.path.join(directory, config.Script)
                let absoluteScriptPath = Node.Exports.path.resolve(scriptPath)

                let importedModule = require absoluteScriptPath

                let x = importedModule?(config.ImportSelector)?RenderBlocks()
                JS.console.log (Helpers.parseReactStatic x)

            | None -> ()
    }

let cwd = Node.Globals.process.cwd()

let liveServerOption =
    jsOptions<LiveServer.Options>(fun o ->
        o.root <- Node.Exports.path.join(cwd, "docs")
    )

LiveServer.liveServer.start(liveServerOption)

open Chokidar

let watcher = chokidar.watch("docsrc/*")

watcher.on(Chokidar.Events.All, (fun event path ->
    match event with
    | Chokidar.Events.Add
    | Chokidar.Events.Change ->
        processMarkdown path
        |> Promise.catchEnd (fun err ->
            printfn "An error occured:\n%A" err
        )
    | Chokidar.Events.Unlink
    | Chokidar.Events.UnlinkDir
    | Chokidar.Events.Ready
    | Chokidar.Events.Raw
    | Chokidar.Events.Error
    | Chokidar.Events.AddDir
    | _ -> ()
))
