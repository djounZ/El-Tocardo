using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ChatToolModeMapper(ILogger<ChatToolModeMapper> logger) : IDomainEntityMapper<ChatToolMode, ChatToolModeDto>
{

    public ChatToolModeDto ToApplication(ChatToolMode domainItem)
    {

        if (domainItem is AutoChatToolMode)
        {
            return new AutoChatToolModeDto();
        }

        if (domainItem is NoneChatToolMode)
        {
            return new NoneChatToolModeDto();
        }

        if (domainItem is RequiredChatToolMode requiredChatToolMode)
        {
            return new RequiredChatToolModeDto( requiredChatToolMode.RequiredFunctionName);
        }
        var notSupportedException = new NotSupportedException($"{domainItem.GetType()} is not supported");
        logger.LogError(notSupportedException, "{@Item} is not handled yet", domainItem);
        throw notSupportedException;
    }
    public ChatToolMode ToDomain(ChatToolModeDto applicationItem)
    {

        if (applicationItem is AutoChatToolModeDto)
        {
            return new AutoChatToolMode();
        }

        if (applicationItem is NoneChatToolModeDto)
        {
            return new NoneChatToolMode();
        }

        if (applicationItem is RequiredChatToolModeDto requiredChatToolMode)
        {
            return new RequiredChatToolMode( requiredChatToolMode.RequiredFunctionName);
        }
        var notSupportedException = new NotSupportedException($"{applicationItem.GetType()} is not supported");
        logger.LogError(notSupportedException, "{@Item} is not handled yet", applicationItem);
        throw notSupportedException;
    }
}