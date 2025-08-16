using System.Net.Http.Headers;

namespace AI.GithubCopilot.Infrastructure.Services;

public sealed class GithubCopilotChatCompletionAuthorizationHandler(GithubCopilotTokenProvider githubCopilotTokenProvider, AiGithubCopilotUserProvider aiGithubCopilotUserProvider) : DelegatingHandler
{

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var githubCopilotTokenAsync = await githubCopilotTokenProvider.GetGithubCopilotTokenAsync(aiGithubCopilotUserProvider.GetCurrentUser(), cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", githubCopilotTokenAsync.Token);
        // Call the inner handler
        return await base.SendAsync(request, cancellationToken);
    }
}
