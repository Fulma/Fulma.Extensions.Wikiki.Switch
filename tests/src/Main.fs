module Tests

open Fable.Import
open Fable.Helpers.React
open Fulma
open Fulma.Extensions.Wikiki

type IExports =
    abstract RenderBlocks : unit -> React.ReactElement


let api =
    { new IExports with
        member this.RenderBlocks () : React.ReactElement =
            div [ ]
                [ Switch.switch [ Switch.Id "switch-1" ]
                    [ str "Toggle setting n°1" ]
                  Switch.switch [ Switch.Id "switch-2" ]
                    [ str "Toggle setting n°2" ] ]
    }
