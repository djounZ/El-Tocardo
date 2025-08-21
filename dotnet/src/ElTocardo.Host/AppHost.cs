using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Configure PostgreSQL database with persistent storage
var postgres = builder.AddPostgres("postgres")
   // .WithDataVolume("el-tocardo-postgres-data") // Persistent volume for database data
    .WithPgAdmin(); // Optional: Add pgAdmin for database management
var database = postgres.AddDatabase("el-tocardo-db-postgres");


// Add MongoDB server resource (containerized via Docker)
var mongo = builder.AddMongoDB("mongodb")
  //  .WithDataVolume() // persists DB data across container restarts
    .WithMongoExpress(); // optional: adds web UI for MongoDB

// Add a database resource within the MongoDB server
var mongodb = mongo.AddDatabase("el-tocardo-db-mongodb");


_ = builder.AddProject<ElTocardo_API>("ElTocardoAPI")
    .WithReference(database)
    .WithReference(mongodb)// Connect the API to the database
    .WaitFor(postgres)
    .WaitFor(mongodb)
    // .WithEnvironment("OllamaOptions:Uri", ollama.GetEndpoint("ollama"))
    // .WaitFor(ollama)
    .WithExternalHttpEndpoints();

builder.Build().Run();
