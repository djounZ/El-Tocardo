using System.Text.Json;
using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;
using AI.GithubCopilot.Infrastructure.Services;
using ElTocardo.Application.Dtos.UserExternalToken;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Queries;
using ElTocardo.Domain.Mediator.Common.Interfaces;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Domain.Models;
using ElTocardo.Infrastructure.EntityFramework.Mediator.UserExternalTokenMediator;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.API.ToMigrate;

public sealed class GithubAccessTokenStore(
    IHttpContextAccessor httpContextAccessor,
    DbSet<UserExternalToken>  userExternalTokensDbSet,
    IQueryHandler<GetUserExternalTokenByKeyQuery, UserExternalTokenItemDto> getByNameQueryHandler,
    UserExternalTokenProtector userExternalTokenProtector) : IGithubAccessTokenResponseDtoProvider
{


    public async Task<GithubAccessTokenResponseDto> GetTokenAsync(CancellationToken cancellationToken)
    {
        var identityName = httpContextAccessor.HttpContext?.User.Identity?.Name ?? string.Empty;
        Result<UserExternalTokenItemDto> userExternalToken;
        if(!string.IsNullOrWhiteSpace(identityName))
        {
            userExternalToken = await getByNameQueryHandler.HandleAsync(new(new(identityName, "github")), cancellationToken);
        }
        else
        {
            userExternalToken = await GetDefaultTokenAsync(cancellationToken);
        }

        if (!userExternalToken.IsSuccess)
        {
            throw userExternalToken.ReadError();
        }

        var userExternalTokenItemDto = userExternalToken.ReadValue();
        return JsonSerializer.Deserialize<GithubAccessTokenResponseDto>(userExternalTokenProtector.Unprotect(userExternalTokenItemDto.Value))!;
    }

    private async Task<Result<UserExternalTokenItemDto>> GetDefaultTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            var userExternalToken = await userExternalTokensDbSet.SingleAsync(cancellationToken);
            return new UserExternalTokenItemDto(userExternalToken.UserId, userExternalToken.Provider,
                userExternalToken.Value, userExternalToken.CreatedAt, userExternalToken.UpdatedAt);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
