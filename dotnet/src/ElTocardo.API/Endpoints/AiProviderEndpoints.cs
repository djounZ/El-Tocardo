using ElTocardo.API.Options;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElTocardo.API.Endpoints;

public static class AiProviderEndpoints
{
    private static string Tags => "AiProvider";
    public static IEndpointRouteBuilder MapAiProviderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("v1/ai-providers",
                async ([FromServices] IAiProviderService service, CancellationToken cancellationToken) => Results.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetAllAiProvider")
            .WithSummary("Get all AI Provider")
            .WithDescription("Returns all AI Provider items")
            .WithTags(Tags)
            .Produces<AiProviderDto[]>()
            .WithOpenApi()
            .CacheOutput(PredefinedOutputCachingPolicy.PerUserVaryByHeaderAuthorizationLongLiving);

        app.MapGet("v1/ai-providers/{provider}",
                async ([FromServices] IAiProviderService service, string provider, CancellationToken cancellationToken) =>
                {
                    var item = await service.GetAsync(provider.Parse<AiProviderEnumDto>(), cancellationToken);
                    return item is not null ? Results.Ok(item) : Results.NotFound();
                })
            .WithName("GetAiProvider")
            .WithSummary("Get AI Provider")
            .WithDescription("Returns a single AI Provider item")
            .WithTags(Tags)
            .Produces<AiProviderDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .CacheOutput(PredefinedOutputCachingPolicy.PerUserVaryByHeaderAuthorizationLongLiving);

        return app;
    }
}
