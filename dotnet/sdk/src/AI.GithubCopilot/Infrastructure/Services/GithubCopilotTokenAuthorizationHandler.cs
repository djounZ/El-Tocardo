using System.Net.Http.Headers;

namespace AI.GithubCopilot.Infrastructure.Services;

public sealed class GithubCopilotTokenAuthorizationHandler(IGithubAccessTokenResponseDtoProvider githubAccessTokenStore) : DelegatingHandler
{

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var githubAccessTokenResponseDto = await githubAccessTokenStore.GetTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Token", githubAccessTokenResponseDto!.AccessToken);
        // Call the inner handler
        return await base.SendAsync(request, cancellationToken);
    }
}
