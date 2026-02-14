using System.Text.Json;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ChatResponseFormatMapper(ILogger<ChatResponseFormatMapper> logger) : IDomainEntityMapper<ChatResponseFormat, ChatResponseFormatDto>
{

    public ChatResponseFormatDto ToApplication(ChatResponseFormat domainItem)
    {
        switch (domainItem)
        {
            case ChatResponseFormatText:
                return new ChatResponseFormatTextDto();
            case ChatResponseFormatJson chatResponseFormatJson:
                return ToChatResponseFormatJsonDto(chatResponseFormatJson);
            default:
                var notSupportedException = new NotSupportedException($"{domainItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "{@Item} is not handled yet", domainItem);
                throw notSupportedException;
        }

    }

    private static ChatResponseFormatDto ToChatResponseFormatJsonDto(ChatResponseFormatJson chatResponseFormatJson)
    {
        var schema = chatResponseFormatJson.Schema;
        var schemaName = chatResponseFormatJson.SchemaName;
        var schemaDescription = chatResponseFormatJson.SchemaDescription;
        return new ChatResponseFormatJsonDto(
            schema?.ToString(), schemaName, schemaDescription);
    }

    public ChatResponseFormat ToDomain(ChatResponseFormatDto applicationItem)
    {
        switch (applicationItem)
        {
            case ChatResponseFormatTextDto:
                return new ChatResponseFormatText();
            case ChatResponseFormatJsonDto formatJson:
                return ToChatResponseFormatJson(formatJson);
            default:
                var notSupportedException = new NotSupportedException($"{applicationItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "{@Item} is not handled yet", applicationItem);
                throw notSupportedException;
        }


    }

    private static ChatResponseFormat ToChatResponseFormatJson(ChatResponseFormatJsonDto formatJson)
    {
        return new ChatResponseFormatJson(
            JsonDocument.Parse(formatJson.Schema ?? "{}").RootElement,
            formatJson.SchemaName,
            formatJson.SchemaDescription
        );
    }
}