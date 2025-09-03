using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;
using AI.GithubCopilot.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ElTocardo.API.ToMigrate;

public sealed class GithubCopilotAccessTokenResponseDtoProvider(
    ILogger<GithubCopilotAccessTokenResponseDtoProvider> logger,
    GithubCopilotAccessTokenResponseDtoHttpClient githubCopilotAccessTokenResponseDtoHttpClient,
    IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor) : IGithubCopilotAccessTokenResponseDtoProvider
{
    public async Task<GithubCopilotAccessTokenResponseDto> GetGithubCopilotTokenAsync(CancellationToken cancellationToken)
    {
        var identityName = httpContextAccessor.HttpContext?.User.Identity?.Name ?? string.Empty;
        var orAddAsync = await GetOrAddAsync(identityName,
            async ()=> await githubCopilotAccessTokenResponseDtoHttpClient.GetGithubCopilotAccessTokenResponseDtoAsync(cancellationToken));

        return  orAddAsync!;
    }


    private async Task<GithubCopilotAccessTokenResponseDto?> GetOrAddAsync(string key, Func<Task<GithubCopilotAccessTokenResponseDto>> func)
    {

        return await memoryCache.GetOrCreateAsync(GetCacheKey(key), entry => CreateAsync(func, entry));

    }
    private async Task<GithubCopilotAccessTokenResponseDto> CreateAsync(Func<Task<GithubCopilotAccessTokenResponseDto>> func, ICacheEntry entry)
    {
        var dto = await func();
        entry.SetAbsoluteExpiration(DateTimeOffset.FromUnixTimeSeconds(dto.ExpiresAt - 300)); // Set expiration 5 minutes before actual expiration
        entry.RegisterPostEvictionCallback(PostEvictionDelegateCallback);
        return dto;
    }

    private void PostEvictionDelegateCallback(object key, object? value, EvictionReason reason, object? state)
    {
        logger.LogInformation(
            "Cache entry with key {@Key} was evicted due to {@Reason} and {@State}.",
            key, reason, state);
    }

    private string GetCacheKey(string user)
    {
        return $"{nameof(GithubCopilotAccessTokenResponseDto)}_{user}";
    }
}
