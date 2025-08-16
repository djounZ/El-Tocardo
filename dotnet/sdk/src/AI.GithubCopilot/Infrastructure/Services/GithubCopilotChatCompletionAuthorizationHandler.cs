namespace AI.GithubCopilot.Infrastructure.Services;

public sealed class GithubCopilotChatAuthorizationHandler() : AbstractAuthorizationHandler
{
    protected override async Task<string> GetParameterAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    protected override string Scheme { get; } = "Bearer";
}
