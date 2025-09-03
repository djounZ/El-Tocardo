using System.Text.Json;
using System.Text.Json.Serialization;
using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;
using AI.GithubCopilot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.GithubCopilot.Infrastructure.Services;

public sealed class GithubCopilotAccessTokenResponseDtoHttpClient(
    ILogger<GithubCopilotAccessTokenResponseDtoHttpClient> logger,
    HttpClient httpClient,
    IOptions<AiGithubOptions> options,
    HttpClientRunner httpClientRunner
)
{



    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };



    private AiGithubOptions Options => options.Value;

    public async Task<GithubCopilotAccessTokenResponseDto> GetGithubCopilotAccessTokenResponseDtoAsync(CancellationToken cancellationToken)
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
    }
}
