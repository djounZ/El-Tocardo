using System.Net.Http.Headers;

namespace AI.GithubCopilot.Infrastructure.Services.ToMigrate;

public sealed class GithubCopilotTokenAuthorizationHandler(GithubAccessTokenStore githubAccessTokenStore) : DelegatingHandler
{

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {

        request.Headers.Authorization = new AuthenticationHeaderValue("Token", githubAccessTokenStore.AccessToken);
        // Call the inner handler
        return await base.SendAsync(request, cancellationToken);
    }
}
