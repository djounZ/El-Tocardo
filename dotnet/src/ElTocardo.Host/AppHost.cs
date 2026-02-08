using Projects;
using static ElTocardo.ServiceDefaults.Constants;

var builder = DistributedApplication.CreateBuilder(args);

// Configure PostgreSQL database with persistent storage
var postgres = builder.AddPostgres(PostgresServerResourceName)
    // .WithDataVolume("el-tocardo-postgres-data") // Persistent volume for database data
    .WithPgAdmin(); // Optional: Add pgAdmin for database management
var sqlDb = postgres.AddDatabase(PostgresDatabaseResourceName);


// Add MongoDB server resource (containerized via Docker)
var mongo = builder.AddMongoDB(MongoDbServerResourceName);
//  .WithDataVolume() // persists DB data across container restarts
mongo.WithMongoExpress(); // optional: adds web UI for MongoDB

// Add a database resource within the MongoDB server
var mongodb = mongo.AddDatabase(MongoDbDatabaseResourceName);


var migrationService = builder.AddProject<ElTocardo_Migrations>(ElTocardoMigrationsProjectResourceName);
var authorizationServer =
    builder.AddProject<ElTocardo_Authorization_Server>(ElTocardoAuthorizationServerProjectResourceName);
var elTocardoApi = builder.AddProject<ElTocardo_API>(ElTocardoApiProjectResourceName);
var elTocardoAssistant =
    builder.AddJavaScriptApp(ElTocardoAssistantResourceName, "../../../ElTocardo.Assistant/", runScriptName: "start");


migrationService
    .WithReference(sqlDb)
    .WithReference(elTocardoAssistant)
    .WaitFor(postgres)
    ;


authorizationServer
    .WithReference(sqlDb)
    .WaitFor(postgres)
    .WaitForCompletion(migrationService)
    ;

elTocardoApi.WithReference(sqlDb)
    .WithReference(mongodb)
    .WithReference(authorizationServer)
    .WithReference(elTocardoAssistant)
    .WaitFor(postgres)
    .WaitFor(mongodb)
    .WaitFor(authorizationServer)
    .WithEnvironment(OpenIddictIssuerEnvironmentVariableName, authorizationServer.GetEndpoint("http"))
    // .WithEnvironment("OllamaOptions:Uri", ollama.GetEndpoint("ollama"))
    // .WaitFor(ollama)
    .WithExternalHttpEndpoints();


elTocardoAssistant.WithReference(elTocardoApi)
    .WithReference(authorizationServer)
    .WaitFor(elTocardoApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
