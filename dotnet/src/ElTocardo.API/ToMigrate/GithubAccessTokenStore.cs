using System.Text.Json;
using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;
using AI.GithubCopilot.Infrastructure.Services;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Infrastructure.EntityFramework.Mediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.UserExternalTokenMediator;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.API.ToMigrate;

public sealed class GithubAccessTokenStore(ILogger<GithubAccessTokenStore> logger, ElTocardoEncryptor elTocardoEncryptor, IConfiguration configuration, DbSet<UserExternalToken> userExternalTokenDbSet, UserExternalTokenProtector userExternalTokenProtector) : IGithubAccessTokenResponseDtoProvider
{

    public async Task SetAccessToken(GithubAccessTokenResponseDto input, CancellationToken cancellationToken)
    {

        try
        {
            var serialize = JsonSerializer.Serialize(input);
            var encryptTokenAesAsync = await elTocardoEncryptor.EncryptTokenAesStringAsync(serialize, cancellationToken);
            configuration[nameof(GithubAccessTokenResponseDto)] = encryptTokenAesAsync;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize GitHub device code response from environment variable.");
        }
    }

    public async Task<bool> IsValidAsync(CancellationToken cancellationToken)
    {
        var githubAccessTokenResponse =
            await GetTokenAsync(
                cancellationToken);
        return !string.IsNullOrEmpty(githubAccessTokenResponse?.AccessToken);
    }


    public async Task<GithubAccessTokenResponseDto?> GetTokenAsync(CancellationToken cancellationToken)
    {
        var userExternalToken = await userExternalTokenDbSet.SingleAsync(cancellationToken);
        return JsonSerializer.Deserialize<GithubAccessTokenResponseDto>(
            userExternalTokenProtector.Unprotect(userExternalToken.Value));
        // try
        // {
        //     var environmentVariable = configuration[nameof(GithubAccessTokenResponseDto)];
        //     if (environmentVariable == null)
        //     {
        //         return null;
        //     }
        //     var decryptTokenAesStringAsync = await elTocardoEncryptor.DecryptTokenAesStringAsync(environmentVariable, cancellationToken);
        //
        //     return JsonSerializer.Deserialize<GithubAccessTokenResponseDto?>(decryptTokenAesStringAsync);
        // }
        // catch (Exception ex)
        // {
        //     logger.LogError(ex, "Failed to deserialize GitHub device code response from environment variable.");
        //     return null;
        // }
    }
}
