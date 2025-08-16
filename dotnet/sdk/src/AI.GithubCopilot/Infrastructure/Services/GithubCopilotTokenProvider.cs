using System.Text.Json;
using System.Text.Json.Serialization;
using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;
using AI.GithubCopilot.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.GithubCopilot.Infrastructure.Services;

public sealed class GithubCopilotTokenProvider(
    ILogger<GithubCopilotTokenProvider> logger,
    HttpClient httpClient,
    IOptions<AiGithubOptions> options,
    IMemoryCache memoryCache,
    HttpClientRunner httpClientRunner)
{
    private AiGithubOptions Options => options.Value;


    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };



    public async Task<GithubCopilotAccessTokenResponseDto> GetGithubCopilotTokenAsync(string user,CancellationToken cancellationToken)
    {
        var orAddAsync = await GetOrAddAsync(user,
            async ()=>
            {
                // Request a new token
                var tokenResponse =
                    await httpClientRunner.SendAndDeserializeAsync<GithubCopilotAccessTokenResponseDto>(
                        httpClient,
                        HttpMethod.Get,
                        Options.CopilotTokenUrl,
                        Options.CopilotTokenHeaders,
                        HttpCompletionOption.ResponseHeadersRead,
                        JsonOptions,
                        cancellationToken,
                        logger);

                return tokenResponse;
            });

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
