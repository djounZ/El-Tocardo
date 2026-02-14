using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services;

public abstract class AbstractChatCompletionsService(
    ILogger<AbstractChatCompletionsService> logger,
    ChatClientProvider clientProvider,
    AiToolsProviderService aiToolsProviderService)
{
    protected ILogger<AbstractChatCompletionsService> Logger { get; } = logger;
    protected ChatClientProvider ClientProvider { get; } = clientProvider;

    protected async Task MapTools(ChatOptionsDto? chatOptionsDto,
        ChatOptions? request, CancellationToken cancellationToken)
    {
        if (chatOptionsDto?.Tools != null)
        {
            request!.Tools = await aiToolsProviderService.GetAiToolsAsync(chatOptionsDto.Tools, cancellationToken);
        }
    }
}
