using AI.GithubCopilot.Infrastructure.Services;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Services;
using Microsoft.Extensions.Logging;
using OllamaSharp;

namespace ElTocardo.Infrastructure.Services.Endpoints;

public sealed class AiProviderEndpointService(ILogger<AiProviderEndpointService> logger, OllamaApiClient ollamaApiClient, GithubCopilotChatCompletion githubCopilotChatCompletion) : IAiProviderEndpointService
{

    public async Task<AiProviderDto[]> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = new List<AiProviderDto>();
        var ollamaTask = GetOllamaAsync(cancellationToken);
        var githubCopilotTask = GetGithubCopilotAsync(cancellationToken);

        AddIfNotNull(result, await ollamaTask);
        AddIfNotNull(result, await githubCopilotTask);

        return [.. result];
    }

    private static void AddIfNotNull(List<AiProviderDto> aiProviderDtos, AiProviderDto? aiProviderDto)
    {
        if (aiProviderDto != null)
        {
            aiProviderDtos.Add(aiProviderDto);
        }
    }

    public async Task<AiProviderDto?> GetAsync(AiProviderEnumDto provider, CancellationToken cancellationToken)
    {
        return provider switch
        {
            AiProviderEnumDto.Ollama =>  await  GetOllamaAsync(cancellationToken),
            AiProviderEnumDto.GithubCopilot => await  GetGithubCopilotAsync(cancellationToken),
            _ => null
        };
    }


    private async Task<AiProviderDto?> GetOllamaAsync(CancellationToken cancellationToken)
    {
        return await GetProviderModels(
            AiProviderEnumDto.Ollama,
        ollamaApiClient.ListLocalModelsAsync,
        modelsResponse=> modelsResponse.Select(model => new AiProviderAiModelDto(model.Name, model.Name))
        ,cancellationToken);
    }
    private async Task<AiProviderDto?> GetGithubCopilotAsync(CancellationToken cancellationToken)
    {
        return await GetProviderModels(
            AiProviderEnumDto.GithubCopilot,
            githubCopilotChatCompletion.GetModelsAsync,
            modelsResponse=> modelsResponse.Data.Select(model => new AiProviderAiModelDto(model.Id, model.Name))
            ,cancellationToken);
    }

    private async Task<AiProviderDto?> GetProviderModels<TProviderModels>(
        AiProviderEnumDto provider,
        Func<CancellationToken, Task<TProviderModels>> getFunc,
        Func<TProviderModels, IEnumerable<AiProviderAiModelDto>> transformFunc,
        CancellationToken token)
    {
        try
        {

            var modelsResponse = await getFunc(token);
            var aiProviderAppModel = new AiProviderDto(
                provider,
                [.. transformFunc(modelsResponse)]);
            return aiProviderAppModel;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get {@Provider} models", provider);
            return null;
        }
    }

}
