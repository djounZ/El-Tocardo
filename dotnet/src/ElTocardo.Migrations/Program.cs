
using ElTocardo.Migrations.Configuration.EntityFramework;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddElTocardoMigrations(builder.Configuration);

var app =  builder
    .Build();

await app.Services.InitializeDatabaseAsync(CancellationToken.None);
//     // .ConfigureElTocardoApiAsync(CancellationToken.None);
// app.Run();
// await app.StopAsync();
