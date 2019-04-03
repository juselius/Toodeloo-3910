module Toodeloo.API

open System
open Microsoft.AspNetCore
open FSharp.Control.Tasks.V2
open Giraffe
open Shared
open DbContext

// let mutable private db = Map.ofList [(1, defaultTodo); (2, defaultTodo)]

let saveEntry next (ctx : Http.HttpContext) =
    task {
        let! todo = ctx.BindJsonAsync<Todo>()
        match addTodo todo with
        | Ok t ->
            return! json (asTodo t) next ctx
        | Error e ->
            ctx.SetStatusCode 500
            return! json e next ctx
    }
    

let loadEntries next ctx =
    task {
        match getTodos () with
        | Ok t -> 
            return! json t next ctx
        | Error e ->
            ctx.SetStatusCode 500
            return! json e next ctx
    }

let webApp : HttpFunc -> Http.HttpContext -> HttpFuncResult =
    choose [
        route "/api/save" >=> saveEntry 
        route "/api/load" >=> loadEntries
    ]
