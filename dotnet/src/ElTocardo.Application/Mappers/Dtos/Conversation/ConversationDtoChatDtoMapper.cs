using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Mappers.Dtos.AI;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Conversation;

public sealed class ConversationDtoChatDtoMapper(AiChatCompletionMapper aiChatCompletionMapper)
{
    public ChatRequestDto MapToChatRequestDto(StartConversationRequestDto startConversationRequestDto)
    {
        return new ChatRequestDto([startConversationRequestDto.InitialUserMessage], startConversationRequestDto.InitialProvider, startConversationRequestDto.Options);
    }

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


    public ChatRequestDto MapToChatRequestDto(ContinueConversationDto continueConversationDto, IList<ChatMessageDto> previousMessages, AiProviderEnumDto? previousProvider = null, ChatOptionsDto? previousOptions = null)
    {
        return new ChatRequestDto([..previousMessages, continueConversationDto.UserMessage], continueConversationDto.Provider ?? previousProvider, continueConversationDto.Options ?? previousOptions);
    }
}
