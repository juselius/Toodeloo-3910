dotnet ef dbcontext scaffold \
    "Host=localhost;Database=toodeloo;Username=postgres;Password=postgres" \
    Npgsql.EntityFrameworkCore.PostgreSQL -o Model -d -v
