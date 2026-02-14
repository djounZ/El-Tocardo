using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ChatMessageMapper(
    IDomainEntityMapper<ChatRole, ChatRoleEnumDto> chatRoleMapper,
    IDomainEntityMapper<AIContent, AiContentDto> aiContentMapper) : IDomainEntityMapper<ChatMessage, ChatMessageDto>
{
    public ChatMessageDto ToApplication(ChatMessage domainItem)
    {
        var contents = domainItem.Contents.Select(aiContentMapper.ToApplication).ToList();
        var role = chatRoleMapper.ToApplication(domainItem.Role);
        var result = new ChatMessageDto(role, contents);
        return result;
    }
    public ChatMessage ToDomain(ChatMessageDto applicationItem)
    {
        var contents = applicationItem.Contents.Select(aiContentMapper.ToDomain).ToList();
        var role = chatRoleMapper.ToDomain(applicationItem.Role);
        var result = new ChatMessage(role, contents);
        return result;
    }
}
