using Projects;
using static ElTocardo.ServiceDefaults.Constants;

var builder = DistributedApplication.CreateBuilder(args);

// Configure PostgreSQL database with persistent storage
var postgres = builder.AddPostgres(PostgresServerResourceName)
   // .WithDataVolume("el-tocardo-postgres-data") // Persistent volume for database data
    .WithPgAdmin(); // Optional: Add pgAdmin for database management
var sqlDb = postgres.AddDatabase(PostgresDatabaseResourceName);


var migrationService = builder.AddProject<ElTocardo_Migrations>(ElTocardoMigrationsProjectResourceName)
        .WithReference(sqlDb)
        .WaitFor(postgres)
    ;
// Add MongoDB server resource (containerized via Docker)
var mongo = builder.AddMongoDB(MongoDbServerResourceName)
  //  .WithDataVolume() // persists DB data across container restarts
    .WithMongoExpress(); // optional: adds web UI for MongoDB

// Add a database resource within the MongoDB server
var mongodb = mongo.AddDatabase(MongoDbDatabaseResourceName);

var authorizationServer = builder.AddProject<ElTocardo_Authorization_Server>(ElTocardoAuthorizationServerProjectResourceName)

        .WithReference(sqlDb)
        .WaitFor(postgres)
        .WaitForCompletion(migrationService)
    ;

_ = builder.AddProject<ElTocardo_API>(ElTocardoApiProjectResourceName)
    .WithReference(sqlDb)
    .WithReference(mongodb)
    .WithReference(authorizationServer)
    .WaitFor(postgres)
    .WaitFor(mongodb)
    .WaitFor(authorizationServer)
    .WithEnvironment(OpenIddictIssuerEnvironmentVariableName, authorizationServer.GetEndpoint("https"))
    // .WithEnvironment("OllamaOptions:Uri", ollama.GetEndpoint("ollama"))
    // .WaitFor(ollama)
    .WithExternalHttpEndpoints();

builder.Build().Run();
