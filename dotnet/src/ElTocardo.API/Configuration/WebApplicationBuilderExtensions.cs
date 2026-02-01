namespace ElTocardo.API.Configuration;

public static class WebApplicationBuilderExtensions
{
    public static IHostApplicationBuilder ConfigureElTocardoApi(this IHostApplicationBuilder builder)
    {

        builder.Services.AddElTocardoApi(builder.Configuration, builder.Environment.ApplicationName);

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
