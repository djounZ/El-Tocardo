using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ChatRoleMapper(ILogger<ChatRoleMapper> logger) : IDomainEntityMapper<ChatRole, ChatRoleEnumDto>
{

    public ChatRoleEnumDto ToApplication(ChatRole domainItem)
    {
        if (domainItem  == ChatRole.System)
        {
            return ChatRoleEnumDto.System;
        }

        if (domainItem == ChatRole.Assistant)
        {
            return ChatRoleEnumDto.Assistant;
        }

        if (domainItem == ChatRole.User)
        {
            return ChatRoleEnumDto.User;
        }

        if (domainItem == ChatRole.Tool)
        {
            return ChatRoleEnumDto.Tool;
        }

        var indexOutOfRangeException = new ArgumentOutOfRangeException(nameof(domainItem), domainItem, null);
        logger.LogError(indexOutOfRangeException, "Failed Conversion of {@Item}", domainItem);
        throw indexOutOfRangeException;
    }
    public ChatRole ToDomain(ChatRoleEnumDto applicationItem)
    {
        switch (applicationItem)
        {
            case ChatRoleEnumDto.System:
                return ChatRole.System;
            case ChatRoleEnumDto.Assistant:
                return ChatRole.Assistant;
            case ChatRoleEnumDto.User:
                return ChatRole.User;
            case ChatRoleEnumDto.Tool:
                return ChatRole.Tool;
            default:
            {
                var indexOutOfRangeException =  new ArgumentOutOfRangeException(nameof(applicationItem), applicationItem, null);
                logger.LogError(indexOutOfRangeException, "Failed Conversion of {@Item}", applicationItem);
                throw indexOutOfRangeException;
            }
        }
    }
}
