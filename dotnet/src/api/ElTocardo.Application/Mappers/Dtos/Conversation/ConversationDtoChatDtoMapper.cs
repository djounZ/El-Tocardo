using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Mappers.Dtos.AI;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Conversation;

public sealed class ConversationDtoChatDtoMapper(AiChatCompletionMapper aiChatCompletionMapper)
{
    public ConversationResponseDto MapToConversationResponseDto(string conversationId, ChatResponse chatResponse)
    {
        var chatResponseDto = aiChatCompletionMapper.MapToChatResponseDto(chatResponse);
        return new ConversationResponseDto(conversationId, chatResponseDto.Messages, chatResponseDto.ModelId, chatResponseDto.CreatedAt, chatResponseDto.FinishReason);
    }

    public ConversationUpdateResponseDto MapToConversationUpdateResponseDto(string conversationId, ChatResponseUpdate update)
    {
        var chatResponseUpdateDto = aiChatCompletionMapper.MapToChatResponseUpdateDto(update);
        return new ConversationUpdateResponseDto(conversationId, chatResponseUpdateDto.Role, chatResponseUpdateDto.Contents, chatResponseUpdateDto.CreatedAt, chatResponseUpdateDto.FinishReason, chatResponseUpdateDto.ModelId);
    }

    public (ChatMessage ChatMessage, ChatOptions? ChatOptions) MapToChatMessageAndOptions(StartConversationRequestDto startConversationRequestDto)
    {
        var chatMessage = aiChatCompletionMapper.MapToChatMessage(startConversationRequestDto.InputMessage);
        var chatOptions = aiChatCompletionMapper.MapToChatOptions(startConversationRequestDto.Options);
        return (chatMessage, chatOptions);
    }


    public (ChatMessage ChatMessage, ChatOptions? ChatOptions) MapToChatMessageAndOptions(ContinueConversationDto continueConversationDto)
    {
        var chatMessage = aiChatCompletionMapper.MapToChatMessage(continueConversationDto.InputMessage);
        var chatOptions = aiChatCompletionMapper.MapToChatOptions(continueConversationDto.Options);
        return (chatMessage, chatOptions);
    }
}
