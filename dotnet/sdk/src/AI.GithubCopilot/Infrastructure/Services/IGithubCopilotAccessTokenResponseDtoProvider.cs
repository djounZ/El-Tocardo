using AI.GithubCopilot.Infrastructure.Dtos.Authorizations;

namespace AI.GithubCopilot.Infrastructure.Services;

public interface IGithubCopilotAccessTokenResponseDtoProvider
{
    public Task<GithubCopilotAccessTokenResponseDto> GetGithubCopilotTokenAsync(CancellationToken cancellationToken);
}
