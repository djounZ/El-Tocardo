using System.Text.Json;
using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;
using AI.GithubCopilot.Infrastructure.Services;
using ElTocardo.Application.Dtos.UserExternalToken;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Queries;
using ElTocardo.Domain.Mediator.Common.Interfaces;

namespace ElTocardo.API.ToMigrate;

public sealed class GithubAccessTokenProvider(
    GithubAccessTokenResponseHttpClient githubAccessTokenResponseHttpClient,
    IQueryHandler<GetUserExternalTokenByKeyQuery, UserExternalTokenItemDto> getByNameQueryHandler,
    ICommandHandler<CreateUserExternalTokenCommand, Guid> createCommandHandler,
    GithubAuthenticator githubAuthenticator)
{
    public async Task GetGithubAccessToken(string userId,
        CancellationToken cancellationToken)
    {
        var userIdExternalToken = await getByNameQueryHandler.HandleAsync(new(new(userId, "github")), cancellationToken);

        if (userIdExternalToken.IsSuccess)
        {
            return;
        }

        await RegisterDeviceAndGetAccessTokenAsync(userId, cancellationToken);
    }

    private async Task RegisterDeviceAndGetAccessTokenAsync(string userId,
        CancellationToken cancellationToken)
    {
        var githubDeviceCodeResponse = await githubAccessTokenResponseHttpClient.RequestDeviceCodeAsync(cancellationToken);
        await githubAuthenticator.AuthenticateAsync(githubDeviceCodeResponse, cancellationToken);
        await GetAndSetAccessTokenAsync(userId, githubDeviceCodeResponse, cancellationToken);
    }


    private async Task GetAndSetAccessTokenAsync(string userId,
        GithubDeviceCodeResponseDto githubDeviceCodeResponseDto,
        CancellationToken cancellationToken)
    {
        var deviceCode = githubDeviceCodeResponseDto.DeviceCode;

        var response = await githubAccessTokenResponseHttpClient.GetGithubAccessTokenResponseAsync(cancellationToken, deviceCode);
        await createCommandHandler.HandleAsync(new(userId, "github", JsonSerializer.Serialize(response)), cancellationToken);
    }
}
