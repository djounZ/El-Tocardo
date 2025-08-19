using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Configure PostgreSQL database with persistent storage
var postgres = builder.AddPostgres("postgres")
   // .WithDataVolume("el-tocardo-postgres-data") // Persistent volume for database data
    .WithPgAdmin(); // Optional: Add pgAdmin for database management

var database = postgres.AddDatabase("el-tocardo-db");


_ = builder.AddProject<ElTocardo_API>("ElTocardoAPI")
    .WithReference(database) // Connect the API to the database
    .WaitFor(postgres)
    // .WithEnvironment("OllamaOptions:Uri", ollama.GetEndpoint("ollama"))
    // .WaitFor(ollama)
    .WithExternalHttpEndpoints();

builder.Build().Run();
