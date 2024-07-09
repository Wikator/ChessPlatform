using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sql-password", secret: true);

var sqlServer = builder.AddContainer("postgres", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", sqlPassword)
    .WithVolume("pgdata", "/var/lib/postgresql/data")
    .WithEndpoint(5432, 5432);

var backend = builder.AddProject<Projects.ChessPlatform_Backend>("backend");

builder.AddProject<Projects.ChessPlatform_Frontend_Server>("frontend")
    .WithExternalHttpEndpoints()
    .WithReference(backend);

builder.Build().Run();