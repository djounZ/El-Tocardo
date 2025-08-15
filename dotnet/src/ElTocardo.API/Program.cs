using ElTocardo.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddElTocardoApi(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // GET {{MCP.WebApi_HostAddress}}/openapi/v1.json
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("DefaultCorsPolicy");

app.MapElTocardoApiEndpoints();
await app.Services.UseElTocardoApiAsync(CancellationToken.None);
app.Run();
