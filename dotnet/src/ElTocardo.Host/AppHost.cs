using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Configure PostgreSQL database with persistent storage
var postgres = builder.AddPostgres("postgres")
   // .WithDataVolume("el-tocardo-postgres-data") // Persistent volume for database data
    .WithPgAdmin(); // Optional: Add pgAdmin for database management
var sqlDb = postgres.AddDatabase("el-tocardo-db-postgres");


var migrationService = builder.AddProject<ElTocardo_Migrations>("migrations")
        .WithReference(sqlDb)
        .WaitFor(postgres)
    ;
// Add MongoDB server resource (containerized via Docker)
var mongo = builder.AddMongoDB("mongodb")
  //  .WithDataVolume() // persists DB data across container restarts
    .WithMongoExpress(); // optional: adds web UI for MongoDB

// Add a database resource within the MongoDB server
var mongodb = mongo.AddDatabase("el-tocardo-db-mongodb");

var authorizationServer = builder.AddProject<ElTocardo_Authorization_Server>("authorization-server")

        .WithReference(sqlDb)
        .WaitFor(postgres)
        .WaitForCompletion(migrationService)
    ;

_ = builder.AddProject<ElTocardo_API>("ElTocardoAPI")
    .WithReference(sqlDb)
    .WithReference(mongodb)
    .WithReference(authorizationServer)
    .WaitFor(postgres)
    .WaitFor(mongodb)
    .WaitFor(authorizationServer)
    .WithEnvironment("OpenIddictIssuer", authorizationServer.GetEndpoint("http"))
    // .WithEnvironment("OllamaOptions:Uri", ollama.GetEndpoint("ollama"))
    // .WaitFor(ollama)
    .WithExternalHttpEndpoints();

builder.Build().Run();
