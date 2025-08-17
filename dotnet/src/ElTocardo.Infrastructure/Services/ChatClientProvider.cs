using ElTocardo.Application.Dtos.Provider;
using ElTocardo.Application.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services;

public sealed class ChatClientProvider(ILogger<ChatClientProvider> logger, IAiProviderService aiProviderService, ChatClientStore chatClientStore)
{

    // should be user specific, but for now we will not filter on user
    public async Task<IChatClient> GetChatClientAsync(AiProviderEnumDto? provider, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving chat client for provider: {Provider}", provider);

        var defaultedProvider = provider ?? AiProviderEnumDto.Ollama;
        var aiProviderDto = await aiProviderService.GetAsync(defaultedProvider, cancellationToken);
        return aiProviderDto == null ?
            throw new UnauthorizedAccessException("Provider not found or not authorized.")
            : chatClientStore.GetMcpChatClient(aiProviderDto.Name);
    }
}
