using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;

namespace AI.GithubCopilot.Infrastructure.Services;

public interface IGithubAccessTokenResponseDtoProvider
{
    public Task<GithubAccessTokenResponseDto?> GetTokenAsync(CancellationToken cancellationToken);
}
