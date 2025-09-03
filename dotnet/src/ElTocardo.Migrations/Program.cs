
using ElTocardo.Migrations.Configuration.EntityFramework;
using ElTocardo.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder
    .AddServiceDefaults()
    .Services.AddElTocardoMigrations(builder.Configuration);

var app =  builder
    .Build();

await app.Services.InitializeDatabaseAsync(CancellationToken.None);
//     // .ConfigureElTocardoApiAsync(CancellationToken.None);
// app.Run();
// await app.StopAsync();
