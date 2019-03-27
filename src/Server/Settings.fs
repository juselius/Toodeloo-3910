module Toodeloo.Settings 

open System.IO
open Microsoft.Extensions.Configuration

let private settings =
    ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory ())
        .AddJsonFile("appsettings.json")
        .Build()
let connString = settings.["ConnectionString"]
