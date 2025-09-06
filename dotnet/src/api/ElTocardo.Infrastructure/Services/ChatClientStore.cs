using AI.GithubCopilot.Domain.Services;
using ElTocardo.Application.Dtos.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OllamaSharp;

namespace ElTocardo.Infrastructure.Services;

public sealed class ChatClientStore(ILoggerFactory loggerFactory,OllamaApiClient ollamaApiClient, GithubCopilotChatClient githubCopilotChatClient)
{

    public IChatClient GetChatClient(AiProviderEnumDto provider)
    {
        return provider switch
        {
            AiProviderEnumDto.Ollama => ollamaApiClient,
            AiProviderEnumDto.GithubCopilot => githubCopilotChatClient,
            _ => throw new NotSupportedException($"Provider {provider} is not supported.")
        };
    }

    public IChatClient GetMcpChatClient(AiProviderEnumDto provider)
    {
        return GetChatClient(provider)
            .AsBuilder()
            .UseFunctionInvocation(loggerFactory)
            .Build();
    }

}
