using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ChatFinishReasonMapper(ILogger<ChatFinishReasonMapper> logger) : IDomainEntityMapper<ChatFinishReason, ChatFinishReasonDto>
{

    public ChatFinishReasonDto ToApplication(ChatFinishReason domainItem)
    {
        if (domainItem == ChatFinishReason.Stop)
        {
            return ChatFinishReasonDto.Stop;
        }

        if (domainItem == ChatFinishReason.Length)
        {
            return ChatFinishReasonDto.Length;
        }

        if (domainItem == ChatFinishReason.ToolCalls)
        {
            return ChatFinishReasonDto.ToolCalls;
        }

        if (domainItem == ChatFinishReason.ContentFilter)
        {
            return ChatFinishReasonDto.ContentFilter;
        }

        var notSupportedException = new NotSupportedException($"{domainItem.GetType()} is not supported");
        logger.LogError(notSupportedException, "Failed {@Item}", domainItem);
        throw notSupportedException;
    }
    
    public ChatFinishReason ToDomain(ChatFinishReasonDto applicationItem)
    {
        switch (applicationItem)
        {
            case ChatFinishReasonDto.Stop:
                return ChatFinishReason.Stop;
            case ChatFinishReasonDto.Length:
                return ChatFinishReason.Length;
            case ChatFinishReasonDto.ToolCalls:
                return ChatFinishReason.ToolCalls;
            case ChatFinishReasonDto.ContentFilter:
                return ChatFinishReason.ContentFilter;
            default:
                var notSupportedException = new NotSupportedException($"{applicationItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "Failed {@Item}", applicationItem);
                throw notSupportedException;
        }

    }
}
