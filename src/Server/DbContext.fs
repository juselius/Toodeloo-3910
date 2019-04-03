module Toodeloo.DbContext

open System
open Microsoft.EntityFrameworkCore
open Shared

let inline internal withDb qry =
    try
        let ctx = new Entity.ToodelooContext (Settings.connString)
        qry ctx |> Ok
    with
    | e -> 
        printfn "withDb: Exception: %s" e.Message
        Error (string e)

let tryMigrate () =
    printf "Running database migrations... "
    match withDb (fun ctx -> ctx.Database.Migrate ()) with
    | Ok _ -> printfn "done."
    | Error e -> printfn "exception in Db.tryMigrate: \n%s" (string e)

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
    withDb f

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
    withDb f


let getItemById id (ctx : Entity.ToodelooContext) = 
        query {
            for t in ctx.Todo do
            where (t.Id = id)
            select t
        } 
        |> Seq.tryHead
let getTodo id = 
    withDb (getItemById id >> Option.map asTodo)

let delTodo id = 
    withDb (fun ctx -> 
        getItemById id ctx 
        |> Option.map (ctx.Remove >> ignore >> ctx.SaveChanges)
    )

let updateTodo id = 
    withDb (fun ctx -> 
        getItemById id ctx 
        |> Option.map (ctx.Update >> ignore >> ctx.SaveChanges)
    )
