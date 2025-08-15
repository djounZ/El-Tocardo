var builder = DistributedApplication.CreateBuilder(args);
_ = builder.AddProject<Projects.ElTocardo_API>("ElTocardoAPI")
    .WithExternalHttpEndpoints();
builder.Build().Run();
