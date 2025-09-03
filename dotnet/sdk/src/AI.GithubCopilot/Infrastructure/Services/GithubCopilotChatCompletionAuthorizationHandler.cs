using System.Net.Http.Headers;

namespace AI.GithubCopilot.Infrastructure.Services;

public sealed class GithubCopilotChatCompletionAuthorizationHandler(IGithubCopilotAccessTokenResponseDtoProvider githubCopilotAccessTokenResponseDtoProvider) : DelegatingHandler
{

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var githubCopilotTokenAsync = await githubCopilotAccessTokenResponseDtoProvider.GetGithubCopilotTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", githubCopilotTokenAsync.Token);
        // Call the inner handler
        return await base.SendAsync(request, cancellationToken);
    }
}
