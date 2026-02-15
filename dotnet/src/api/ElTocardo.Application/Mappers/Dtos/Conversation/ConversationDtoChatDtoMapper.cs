using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Conversation;

public sealed class ConversationDtoChatDtoMapper(
    IDomainEntityMapper<ChatOptions, ChatOptionsDto> chatOptionsMapper,
    IDomainEntityMapper<ChatMessage, ChatMessageDto> chatMessageMapper,
    IDomainEntityMapper<ChatResponse, ChatResponseDto> chatResponseMapper,
    IDomainEntityMapper<ChatResponseUpdate, ChatResponseUpdateDto> chatResponseUpdateMapper)
{
    public ConversationResponseDto MapToConversationResponseDto(string conversationId, ChatResponse chatResponse)
    {
        var chatResponseDto = chatResponseMapper.ToApplication(chatResponse);
        return new ConversationResponseDto(conversationId, chatResponseDto.Messages, chatResponseDto.ModelId, chatResponseDto.CreatedAt, chatResponseDto.FinishReason);
    }

    public ConversationUpdateResponseDto MapToConversationUpdateResponseDto(string conversationId, ChatResponseUpdate update)
    {
        var chatResponseUpdateDto = chatResponseUpdateMapper.ToApplication(update);
        return new ConversationUpdateResponseDto(conversationId, chatResponseUpdateDto.Role, chatResponseUpdateDto.Contents, chatResponseUpdateDto.CreatedAt, chatResponseUpdateDto.FinishReason, chatResponseUpdateDto.ModelId);
    }

    public (ChatMessage ChatMessage, ChatOptions? ChatOptions) MapToChatMessageAndOptions(StartConversationRequestDto startConversationRequestDto)
    {
        var chatMessage = chatMessageMapper.ToDomain(startConversationRequestDto.InputMessage);
        var chatOptions = chatOptionsMapper.ToDomainNullable(startConversationRequestDto.Options);
        return (chatMessage, chatOptions);
    }


    public (ChatMessage ChatMessage, ChatOptions? ChatOptions) MapToChatMessageAndOptions(ContinueConversationDto continueConversationDto)
    {
        var chatMessage = chatMessageMapper.ToDomain(continueConversationDto.InputMessage);
        var chatOptions = chatOptionsMapper.ToDomainNullable(continueConversationDto.Options);
        return (chatMessage, chatOptions);
    }
}
