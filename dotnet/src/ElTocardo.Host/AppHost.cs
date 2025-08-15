using Projects;

var builder = DistributedApplication.CreateBuilder(args);
_ = builder.AddProject<ElTocardo_API>("ElTocardoAPI")
    .WithExternalHttpEndpoints();
builder.Build().Run();
