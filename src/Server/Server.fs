module Toodeloo.Server

open System
open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

open FSharp.Control.Tasks.V2
open Giraffe
open Shared
open System
open Microsoft.EntityFrameworkCore

let tryGetEnv = 
    System.Environment.GetEnvironmentVariable 
    >> function null | "" -> None | x -> Some x

let publicPath = Path.GetFullPath "../Client/public"

let port = 
    "SERVER_PORT" 
    |> tryGetEnv 
    |> Option.map uint16 
    |> Option.defaultValue 8085us

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

let webApp =
    choose [
        route "/api/save" >=> fun next ctx ->
            task {
                let! todo = ctx.BindJsonAsync<Todo>()
                match addTodo todo with
                | Ok t ->
                    printfn "%A" t
                    return! json (asTodo t) next ctx
                | Error e ->
                    ctx.SetStatusCode 500
                    return! json e next ctx
            }
        route "/api/load" >=> fun next ctx ->
            task {
                match getTodos () with
                | Ok t -> 
                    printfn "%A" t
                    return! json t next ctx
                | Error e ->
                    ctx.SetStatusCode 500
                    return! json e next ctx
            }
    ]

let configureApp (app : IApplicationBuilder) =
    app.UseDefaultFiles()
       .UseStaticFiles()
       .UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore
    services.AddSingleton<Giraffe.Serialization.Json.IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer()) |> ignore


let runServer () =
    WebHost
        .CreateDefaultBuilder()
        .UseWebRoot(publicPath)
        .UseContentRoot(publicPath)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .UseUrls("http://0.0.0.0:" + port.ToString() + "/")
        .Build()
        .Run()

[<EntryPoint>]
let main args =
    DbContext.tryMigrate ()
    runServer ()
    0
