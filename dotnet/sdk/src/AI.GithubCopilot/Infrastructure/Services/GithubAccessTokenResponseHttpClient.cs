using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;
using AI.GithubCopilot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.GithubCopilot.Infrastructure.Services;

public sealed class GithubAccessTokenResponseHttpClient(
    ILogger<GithubAccessTokenResponseHttpClient> logger,
    HttpClient httpClient,
    IOptions<AiGithubOptions> options,
    HttpClientRunner httpClientRunner
)
{

    private AiGithubOptions Options => options.Value;



    public async Task<GithubDeviceCodeResponseDto> RequestDeviceCodeAsync(
        CancellationToken cancellationToken)
    {
        var request = new GithubDeviceCodeRequestDto
        {
            ClientId = Options.ClientId, Scope = Options.DeviceScope
        };

        var response =
            await httpClientRunner.SendAndDeserializeAsync<GithubDeviceCodeRequestDto, GithubDeviceCodeResponseDto>(
                request,
                httpClient,
                HttpMethod.Post,
                Options.DeviceCodeUrl,
                Options.DeviceCodeHeaders,
                HttpCompletionOption.ResponseHeadersRead,
                null,
                cancellationToken,
                logger);
        return response;
    }
    public async Task<GithubAccessTokenResponseDto> GetGithubAccessTokenResponseAsync(CancellationToken cancellationToken, string deviceCode)
    {
        var request = new GithubAccessTokenRequestDto
        {
            ClientId = Options.ClientId,
            DeviceCode = deviceCode,
            TokenGrantType = Options.GrantType
        };

        var response =
            await httpClientRunner.SendAndDeserializeAsync<GithubAccessTokenRequestDto, GithubAccessTokenResponseDto>(
                request,
                httpClient,
                HttpMethod.Post,
                Options.TokenUrl,
                Options.TokenHeaders,
                HttpCompletionOption.ResponseHeadersRead,
                null,
                cancellationToken,
                logger);
        return response;
    }

}
