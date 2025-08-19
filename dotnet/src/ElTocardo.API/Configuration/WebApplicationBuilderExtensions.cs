using Microsoft.EntityFrameworkCore;

namespace ElTocardo.API.Configuration;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureElTocardoApi(this WebApplicationBuilder builder)
    {

        builder.Services.AddElTocardoApi(builder.Configuration, builder.Environment.ApplicationName);
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

        return builder;
    }
}
