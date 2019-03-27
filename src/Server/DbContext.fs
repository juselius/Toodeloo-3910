module Toodeloo.DbContext

open Microsoft.EntityFrameworkCore

let inline internal withDb qry =
    try
        let ctx = new Entity.ToodelooContext (Settings.connString)
        qry ctx |> Ok
    with
    | e -> Error (string e)

let tryMigrate () =
    printf "Running database migrations... "
    match withDb (fun ctx -> ctx.Database.Migrate ()) with
    | Ok _ -> printfn "done."
    | Error e -> printfn "exception in Db.tryMigrate: \n%s" (string e)
