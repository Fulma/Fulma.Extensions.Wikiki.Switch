module Tests

open Fable.Import
open Fulma

type IExports =
    abstract RenderBlocks : unit -> React.ReactElement


let api =
    { new IExports with
        member this.RenderBlocks(): React.ReactElement = }
