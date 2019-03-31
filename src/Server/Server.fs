module Toodeloo.Server

open System
open System.IO

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

open Giraffe
open FSharp.Control.Tasks.V2

let tryGetEnv = 
    System.Environment.GetEnvironmentVariable 
    >> function null | "" -> None | x -> Some x

let publicPath = Path.GetFullPath "../Client/public"

let port = 
    "SERVER_PORT" 
    |> tryGetEnv 
    |> Option.map uint16 
    |> Option.defaultValue 8085us

open Shared

let defaultTodo = { 
        title = "Default"
        description = ""
        priority = 0
        due = None
        }

let configureApp (app : IApplicationBuilder) =
    app.UseDefaultFiles()
       .UseStaticFiles()
       .UseGiraffe API.webApp

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
