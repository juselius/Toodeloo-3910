# Setting up Entity Framework Core for an existing database

## Scaffold
```sh
dotnet ef dbcontext scaffold \
    "Host=localhost;Database=toodeloo;Username=postgres;Password=postgres" \
    Npgsql.EntityFrameworkCore.PostgreSQL -o Toodeloo -d -v
```

## Edit

## Add initial migration

```sh
dotnet ef migrations add Init -c ToodelooContext -o Migrations
```

