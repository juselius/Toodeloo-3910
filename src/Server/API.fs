module Toodeloo.API

open System
open Microsoft.AspNetCore
open FSharp.Control.Tasks.V2
open Giraffe
open Shared

let mutable private db = Map.ofList [(1, defaultTodo); (2, defaultTodo)]

let addTodo item = 
    let entry = Entity.Todo()
    entry.Title <- item.title
    entry.Description <- item.description
    entry.Priority <- Nullable item.priority 
    entry.Due <- Option.toNullable item.due
    let f (ctx : Entity.ToodelooContext) = 
        ctx.Add entry |> ignore
        ctx.SaveChanges () |> ignore
        entry
    DbContext.withDb f

let asTodo (t : Entity.Todo) = 
    {
        title = t.Title
        description = t.Description
        priority = t.Priority.Value
        due = Option.ofNullable t.Due
    }

let getTodos () = 
    let f (ctx : Entity.ToodelooContext) = 
        query {
            for t in ctx.Todo do
                select t
        } |> Seq.map asTodo
    DbContext.withDb f

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
