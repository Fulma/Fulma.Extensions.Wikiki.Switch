module Main

open Fable.Import

open Fable.Core
open Fulma
open Fable.Helpers.React
open Fable.Helpers.React.Props

open System.Text.RegularExpressions

// open Renderer

// FromMarkdown.renderNormal (Route.Index) "Index.md"

// FromMarkdown.renderWithTOC (Route.Json.v1) "Json/v1.md"
// FromMarkdown.renderWithTOC (Route.Json.v2) "Json/v2.md"
// FromMarkdown.renderWithTOC (Route.Json.v3) "Json/v3.md"

// FromMarkdown.renderWithTOC (Route.Elmish.Debouncer) "Elmish/Debouncer.md"
// FromMarkdown.renderWithTOC (Route.Elmish.Toast.Docs) "Elmish/Toast_docs.md"
// FromMarkdown.renderNormal (Route.Elmish.Toast.Demo) "Elmish/Toast_demo.md"
// FromMarkdown.renderWithTOC (Route.Elmish.FormBuilder) "Elmish/FormBuilder.md"


printfn "Hello world2"

let output = Node.Exports.path.join(Node.Globals.__dirname, "..", "..", "..", "docs", "index.html")

printfn "%s" output

type PageData =
    { Title : string
      PageContent : string }

let header =
    div [ Class "page-header" ]
        [ Hero.hero [ Hero.Color IsLight ]
            [ Hero.body [ Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                [ div [ Class "intersection-observer" ]
                    [ ]
                  Heading.h2 [ Heading.IsSpaced ]
                    [ str "Fulma.Extensions.Wikiki.Switch" ]
                  Heading.p [ Heading.IsSubtitle
                              Heading.Is5
                              Heading.Modifiers [ Modifier.TextWeight TextWeight.Light ] ]
                    [ str "Wrapper on top of bulma-switch providing support for displaying the classic checkbox as a switch button" ] ] ] ]

let sidebarHeader =
    div [ Style [ Display "flex"
                  JustifyContent "center" ] ]
        [ Image.image [ Image.Is128x128 ]
            [ img [ Src "assets/logo_transparent.svg"
                    Style [ Height "100%" ] ] ] ]

let renderPage tocContent pageContent =
    Columns.columns [ Columns.IsGapless ]
        [ Column.column [ ]
            [ Section.section [ ]
                [ Content.content [ ]
                    [ div [ DangerouslySetInnerHTML { __html =  pageContent } ] [ ] ] ] ]
          Column.column [ Column.Width (Screen.All, Column.Is3)
                          Column.Modifiers [ Modifier.IsHidden (Screen.Touch, true) ]
                          Column.Props [ DangerouslySetInnerHTML { __html =  tocContent } ] ]
            [ ] ]

let renderBadge (href : string) (badgeUrl : string)=
    a [ Href href ]
        [ img [ Src badgeUrl ] ]

let badges =
    Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
        [ p [ ]
            [ renderBadge "" "https://badgen.net/badge/Version/1.0.0/green" ]
          p [ ]
            [ renderBadge "" "https://badgen.net/badge//npm/blue?icon=npm"
              whitespace
              renderBadge "" "https://badgen.net/badge//github/blue?icon=github" ] ]

let tocRegex = new Regex("""(<nav class="toc-container">.*<\/nav>)""")

let pageTemplate (pageData : PageData) =
    let tocContent =
        let result = tocRegex.Match(pageData.PageContent)

        if result.Success then
            result.Value
        else
            """<nav class="toc-container"><\/nav>"""

    let pageContent =
        tocRegex.Replace(pageData.PageContent, "")

    html [ ]
        [ head [ ]
            [ title [ ]
                [ str pageData.Title ]
              link [ Rel "stylesheet"
                     Type "text/css"
                     Href "style.css" ]
              script [ Src "https://polyfill.app/api/polyfill?features=scroll-behavior,intersection-observer" ]
                [ ] ]
          body [ ]
            [ Columns.columns [ Columns.IsGapless
                                Columns.CustomClass "page-content" ]
                [ Column.column [ Column.Width (Screen.All, Column.Is2)
                                  Column.CustomClass "sidebar" ]
                    [ Section.section [ ]
                        [ sidebarHeader
                          badges ] ]
                  Column.column [ Column.CustomClass "main-content"
                                  // We need to set ScrollBehavior as inline style
                                  // for the polyfill to detect it and apply
                                  // Needed for IE11 + Safari for example
                                  Column.Props [ Style [ ScrollBehavior "smooth" ] ] ]
                    [ header
                      renderPage tocContent pageContent ] ] ] ]
    |> Helpers.parseReactStatic
    |> Helpers.writeFile output

let indexMD = Node.Exports.path.join(Node.Globals.__dirname, "..", "..", "..", "docsrc", "index.md")

promise {
    let! content =
        Helpers.readFile indexMD
        |> Helpers.makeHtml

    pageTemplate
        { Title = "Fulma.Extensions.Wikiki.Switch | Home"
          PageContent = content }
}
|> Promise.start
