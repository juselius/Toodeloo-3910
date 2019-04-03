module Toodeloo.API

open System
open Microsoft.AspNetCore
open FSharp.Control.Tasks.V2
open Giraffe
open Shared
open DbContext

let saveEntry next (ctx : Http.HttpContext) =
    task {
        let! todo = ctx.BindJsonAsync<Todo>()
        match addTodo todo with
        | Ok t -> return! json (t.Id, asTodo t) next ctx
        | Error e ->
            ctx.SetStatusCode 500
            return! json e next ctx
    }
    
let loadEntries () =
    match getTodos () with
    | Ok t -> json t
    | Error e -> json e >=> setStatusCode 500

let loadEntry id =
    match getTodo id with
    | Ok t ->  json t 
    | Error e -> json e >=> setStatusCode 500

let deleteEntry id =
    match delTodo id with
    | Ok t -> json t 
    | Error e -> json e >=> setStatusCode 500

let updateEntry id =
    bindJson<Todo> (fun t -> 
        printfn "%i %A" id t 
        match updateTodo id t with
        | Ok r -> json r
        | Error e -> json e >=> setStatusCode 500
    ) 

let webApp : HttpFunc -> Http.HttpContext -> HttpFuncResult =
    choose [
        route "/api/save" >=> warbler (fun _ -> saveEntry)
        route "/api/load" >=> warbler (fun _ -> loadEntries ())
        routef "/api/load/%i" loadEntry 
        routef "/api/delete/%i" deleteEntry 
        routef "/api/update/%i" updateEntry 
    ]
