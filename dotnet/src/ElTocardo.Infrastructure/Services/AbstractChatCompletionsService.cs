using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services;

public abstract class AbstractChatCompletionsService(
    ILogger<AbstractChatCompletionsService> logger,
    AiChatCompletionMapper aiChatCompletionMapper,
    ChatClientProvider clientProvider,
    AiToolsProviderService aiToolsProviderService)
{
    protected ILogger<AbstractChatCompletionsService> Logger { get; } = logger;
    protected AiChatCompletionMapper AiChatCompletionMapper { get; } = aiChatCompletionMapper;
    protected ChatClientProvider ClientProvider { get; } = clientProvider;

    protected async Task MapTools(ChatOptionsDto? chatOptionsDto,
        AiChatCompletionMapper.AiChatClientRequest request, CancellationToken cancellationToken)
    {
        if (chatOptionsDto?.Tools != null)
        {
            request.Options!.Tools = await aiToolsProviderService.GetAiToolsAsync(chatOptionsDto.Tools, cancellationToken);
        }
    }
}
