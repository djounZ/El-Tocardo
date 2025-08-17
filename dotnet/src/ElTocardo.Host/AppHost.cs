using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// var ollama = builder.AddContainer("ollama", "ollama/ollama:latest")
//     .WithEndpoint(port: 11435, targetPort: 11434, name:"ollama", scheme:"http")
//     .WithEnvironment("OLLAMA_MODELS", "driaforall/tiny-agent-a:0.5b") // Download llama2 at startup
//     .WithVolume("ollama_data", "/root/.ollama");



_ = builder.AddProject<ElTocardo_API>("ElTocardoAPI")
    // .WithEnvironment("OllamaOptions:Uri", ollama.GetEndpoint("ollama"))
    // .WaitFor(ollama)
    .WithExternalHttpEndpoints();
builder.Build().Run();
