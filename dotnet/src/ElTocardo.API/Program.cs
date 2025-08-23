using ElTocardo.API.Configuration;

var builder = WebApplication
    .CreateBuilder(args)
    .ConfigureElTocardoApi();

var app = await builder
    .Build()
    .ConfigureElTocardoApiAsync(CancellationToken.None);
await app.InitializeIfFirstTimeAsync(CancellationToken.None);
app.Run();
