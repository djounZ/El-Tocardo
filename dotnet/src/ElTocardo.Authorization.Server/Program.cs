using ElTocardo.Authorization.Server.Configuration;
using ElTocardo.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder
    .AddServiceDefaults()
    .Services.AddElTocardoAuthorizationServer(builder.Configuration);

var app = builder
    .Build()
    .MapDefaultEndpoints()
    ;
app.ConfigureElTocardoAuthorizationServer();

app.Run();
