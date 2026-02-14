using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ChatOptionsMapper(
    IDomainEntityMapper<ReasoningOptions, ReasoningOptionsDto> reasoningOptionsMapper,
    IDomainEntityMapper<ChatResponseFormat, ChatResponseFormatDto> chatResponseFormatMapper,
    IDomainEntityMapper<ChatToolMode, ChatToolModeDto> chatToolModeMapper,
    IDomainEntityMapper<AITool, AiToolDto> aiToolMapper) : IDomainEntityMapper<ChatOptions, ChatOptionsDto>
{

    public ChatOptionsDto ToApplication(ChatOptions domainItem)
    {

        var reasoning = reasoningOptionsMapper.ToApplicationNullable(domainItem.Reasoning);
        var responseFormat = chatResponseFormatMapper.ToApplicationNullable(domainItem.ResponseFormat);
        var toolMode = chatToolModeMapper.ToApplicationNullable(domainItem.ToolMode);
        var tools = domainItem.Tools?.Select(aiToolMapper.ToApplication).ToList();

        var toolsDic = tools == null ? null : new Dictionary<string, IList<AiToolDto>>(){{string.Empty, tools}};



        var allowMultipleToolCalls = domainItem.AllowMultipleToolCalls;
        var conversationId = domainItem.ConversationId;
        var frequencyPenalty = domainItem.FrequencyPenalty;
        var instructions = domainItem.Instructions;
        var maxOutputTokens = domainItem.MaxOutputTokens;
        var modelId = domainItem.ModelId;
        var presencePenalty = domainItem.PresencePenalty;
        var seed = domainItem.Seed;
        var temperature = domainItem.Temperature;
        var topK = domainItem.TopK;
        var topP = domainItem.TopP;
        var stopSequences = domainItem.StopSequences;

        var result = new ChatOptionsDto(
            conversationId,
            instructions,
            temperature,
            maxOutputTokens,
            topP,
            topK,
            frequencyPenalty,
            presencePenalty,
            seed,
            reasoning,
            responseFormat,
            modelId,
            stopSequences,
            allowMultipleToolCalls,
            toolMode,
            toolsDic
        );

        return result;
    }
    public ChatOptions ToDomain(ChatOptionsDto applicationItem)
    {

        var reasoning = reasoningOptionsMapper.ToDomainNullable(applicationItem.Reasoning);
        var responseFormat = chatResponseFormatMapper.ToDomainNullable(applicationItem.ResponseFormat);
        var toolModeDto = chatToolModeMapper.ToDomainNullable(applicationItem.ToolMode);
        var tools = applicationItem.Tools?.Values.SelectMany(m=>m).Select(aiToolMapper.ToDomain).ToList();




        var allowMultipleToolCalls = applicationItem.AllowMultipleToolCalls;
        var conversationId = applicationItem.ConversationId;
        var frequencyPenalty = applicationItem.FrequencyPenalty;
        var instructions = applicationItem.Instructions;
        var maxOutputTokens = applicationItem.MaxOutputTokens;
        var modelId = applicationItem.ModelId;
        var presencePenalty = applicationItem.PresencePenalty;
        var seed = applicationItem.Seed;
        var temperature = applicationItem.Temperature;
        var topK = applicationItem.TopK;
        var topP = applicationItem.TopP;
        var stopSequences = applicationItem.StopSequences;

        var result = new ChatOptions
        {
            ConversationId = conversationId,
            Instructions = instructions,
            Temperature =  temperature,
            MaxOutputTokens = maxOutputTokens,
            TopP = topP,
            TopK = topK,
            FrequencyPenalty = frequencyPenalty,
            PresencePenalty = presencePenalty,
            Seed = seed,
            Reasoning = reasoning,
            ResponseFormat = responseFormat,
            ModelId = modelId,
            StopSequences = stopSequences,
            AllowMultipleToolCalls = allowMultipleToolCalls,
            ToolMode = toolModeDto,
            Tools = tools
        };

        return result;
    }
}