using AI.GithubCopilot.Domain.Services;
using ElTocardo.Application.Dtos.Provider;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace ElTocardo.Infrastructure.Services;

public sealed class ChatClientStore(OllamaApiClient ollamaApiClient, GithubCopilotChatClient githubCopilotChatClient)
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

}