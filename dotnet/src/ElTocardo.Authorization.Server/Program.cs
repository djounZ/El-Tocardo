using ElTocardo.Authorization.Server.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddElTocardoAuthorizationServer(builder.Configuration);

var app = builder.Build();
app.ConfigureElTocardoAuthorizationServer();
app.MapGet("/", () => "Hello World!");

app.Run();
