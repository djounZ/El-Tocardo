using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;
using AI.GithubCopilot.Infrastructure.Services;

namespace ElTocardo.API.ToMigrate;

public sealed class GithubAccessTokenStore(EncryptedEnvironment encryptedEnvironment) : IGithubAccessTokenResponseDtoProvider
{

    public async Task SetAccessToken(GithubAccessTokenResponseDto input, CancellationToken cancellationToken)
    {
        await encryptedEnvironment.SetEnvironmentVariableAsync(nameof(GithubAccessTokenResponseDto),
            input, cancellationToken);
    }

    public async Task<bool> IsValidAsync(CancellationToken cancellationToken)
    {
        var githubAccessTokenResponse =
            await GetTokenAsync(
                cancellationToken);
        return !string.IsNullOrEmpty(githubAccessTokenResponse?.AccessToken);
    }


    public async Task<GithubAccessTokenResponseDto?> GetTokenAsync(CancellationToken cancellationToken){

        return await encryptedEnvironment.GetEnvironmentVariableAsync<GithubAccessTokenResponseDto>(
            nameof(GithubAccessTokenResponseDto),
            cancellationToken);
    }
}
