using ElTocardo.API.Configuration;
using ElTocardo.ServiceDefaults;

var builder = WebApplication
    .CreateBuilder(args);

builder
    .AddServiceDefaults()
    .ConfigureElTocardoApi();

var app = await builder
    .Build()
    .MapDefaultEndpoints()
    .ConfigureElTocardoApiAsync(CancellationToken.None);
app.Run();
