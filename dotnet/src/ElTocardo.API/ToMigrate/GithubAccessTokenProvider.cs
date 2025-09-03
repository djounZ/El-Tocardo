using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;
using AI.GithubCopilot.Infrastructure.Services;

namespace ElTocardo.API.ToMigrate;

public sealed class GithubAccessTokenProvider(
    GithubAccessTokenResponseHttpClient githubAccessTokenResponseHttpClient,
    IGithubAccessTokenResponseDtoProvider githubAccessTokenResponseDtoProvider,
    GithubAuthenticator githubAuthenticator)
{
    private readonly GithubAccessTokenStore _githubAccessTokenStore = (GithubAccessTokenStore) githubAccessTokenResponseDtoProvider;
    public async Task GetGithubAccessToken(
        CancellationToken cancellationToken)
    {
        if (await _githubAccessTokenStore.IsValidAsync(cancellationToken))
        {
            return;
        }

        await RegisterDeviceAndGetAccessTokenAsync(cancellationToken);
    }

    private async Task RegisterDeviceAndGetAccessTokenAsync(
        CancellationToken cancellationToken)
    {
        var githubDeviceCodeResponse = await githubAccessTokenResponseHttpClient.RequestDeviceCodeAsync(cancellationToken);
        await githubAuthenticator.AuthenticateAsync(githubDeviceCodeResponse, cancellationToken);
        await GetAndSetAccessTokenAsync(githubDeviceCodeResponse, cancellationToken);
    }


    private async Task GetAndSetAccessTokenAsync(
        GithubDeviceCodeResponseDto githubDeviceCodeResponseDto,
        CancellationToken cancellationToken)
    {
        var deviceCode = githubDeviceCodeResponseDto.DeviceCode;

        var response = await githubAccessTokenResponseHttpClient.GetGithubAccessTokenResponseAsync(cancellationToken, deviceCode);
        await _githubAccessTokenStore.SetAccessToken(response, cancellationToken);
    }
}
