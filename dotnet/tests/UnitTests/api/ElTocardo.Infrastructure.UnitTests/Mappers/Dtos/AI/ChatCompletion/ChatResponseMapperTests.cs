using ElTocardo.Application.Dtos.Microsoft.Extensions.AI;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ChatResponseMapperTests
{
    private readonly Mock<IDomainEntityMapper<ChatMessage, ChatMessageDto>> _chatMessageMapperMock;
    private readonly Mock<IDomainEntityMapper<ChatFinishReason, ChatFinishReasonDto>> _chatFinishReasonMapperMock;
    private readonly Mock<IDomainEntityMapper<UsageDetails, UsageDetailsDto>> _usageDetailsMapperMock;
    private readonly IDomainEntityMapper<ChatResponse, ChatResponseDto> _mapper;

    public ChatResponseMapperTests()
    {
        _chatMessageMapperMock = new Mock<IDomainEntityMapper<ChatMessage, ChatMessageDto>>();
        _chatFinishReasonMapperMock = new Mock<IDomainEntityMapper<ChatFinishReason, ChatFinishReasonDto>>();
        _usageDetailsMapperMock = new Mock<IDomainEntityMapper<UsageDetails, UsageDetailsDto>>();
        
        _mapper = new ChatResponseMapper(
            _chatMessageMapperMock.Object,
            _chatFinishReasonMapperMock.Object,
            _usageDetailsMapperMock.Object
        );
    }

    [Fact]
    public void ToApplication_AllPropertiesSet_MapsCorrectly()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var message = new ChatMessage(ChatRole.Assistant, [new TextContent("Response")]);
        var usage = new UsageDetails
        {
            InputTokenCount = 10,
            OutputTokenCount = 20,
            TotalTokenCount = 30
        };

        var domain = new ChatResponse
        {
            Messages = [message],
            ResponseId = "resp-123",
            ConversationId = "conv-456",
            ModelId = "gpt-4",
            CreatedAt = createdAt,
            FinishReason = ChatFinishReason.Stop,
            Usage = usage
        };

        var messageDto = new ChatMessageDto(ChatRoleEnumDto.Assistant, [new TextContentDto(null, "Response")]);
        var usageDto = new UsageDetailsDto(10, 20, 30, null, null, null);

        _chatMessageMapperMock
            .Setup(m => m.ToApplication(message))
            .Returns(messageDto);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToApplication(ChatFinishReason.Stop))
            .Returns(ChatFinishReasonDto.Stop);
        _usageDetailsMapperMock
            .Setup(m => m.ToApplication(usage))
            .Returns(usageDto);

        var result = _mapper.ToApplication(domain);

        Assert.Single(result.Messages);
        Assert.Equal(messageDto, result.Messages[0]);
        Assert.Equal("resp-123", result.ResponseId);
        Assert.Equal("conv-456", result.ConversationId);
        Assert.Equal("gpt-4", result.ModelId);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal(ChatFinishReasonDto.Stop, result.FinishReason);
        Assert.Equal(usageDto, result.Usage);
        _chatMessageMapperMock.Verify(m => m.ToApplication(message), Times.Once);
        _chatFinishReasonMapperMock.Verify(m => m.ToApplication(ChatFinishReason.Stop), Times.Once);
        _usageDetailsMapperMock.Verify(m => m.ToApplication(usage), Times.Once);
    }

    [Fact]
    public void ToApplication_UsageNull_MapsCorrectly()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var message = new ChatMessage(ChatRole.User, [new TextContent("Request")]);

        var domain = new ChatResponse
        {
            Messages = [message],
            ResponseId = "resp-789",
            ConversationId = "conv-012",
            ModelId = "gpt-3.5",
            CreatedAt = createdAt,
            FinishReason = ChatFinishReason.Length,
            Usage = null
        };

        var messageDto = new ChatMessageDto(ChatRoleEnumDto.User, [new TextContentDto(null, "Request")]);

        _chatMessageMapperMock
            .Setup(m => m.ToApplication(message))
            .Returns(messageDto);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToApplication(ChatFinishReason.Length))
            .Returns(ChatFinishReasonDto.Length);

        var result = _mapper.ToApplication(domain);

        Assert.Null(result.Usage);
        Assert.Equal(ChatFinishReasonDto.Length, result.FinishReason);
        _usageDetailsMapperMock.Verify(m => m.ToApplication(It.IsAny<UsageDetails>()), Times.Never);
    }

    [Fact]
    public void ToApplication_MultipleMessages_MapsCorrectly()
    {
        var message1 = new ChatMessage(ChatRole.User, [new TextContent("Question")]);
        var message2 = new ChatMessage(ChatRole.Assistant, [new TextContent("Answer")]);

        var domain = new ChatResponse
        {
            Messages = [message1, message2],
            ResponseId = "resp-multi",
            ConversationId = "conv-multi",
            ModelId = "gpt-4",
            CreatedAt = DateTimeOffset.UtcNow,
            FinishReason = ChatFinishReason.Stop,
            Usage = null
        };

        var messageDto1 = new ChatMessageDto(ChatRoleEnumDto.User, [new TextContentDto(null, "Question")]);
        var messageDto2 = new ChatMessageDto(ChatRoleEnumDto.Assistant, [new TextContentDto(null, "Answer")]);

        _chatMessageMapperMock.Setup(m => m.ToApplication(message1)).Returns(messageDto1);
        _chatMessageMapperMock.Setup(m => m.ToApplication(message2)).Returns(messageDto2);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToApplication(ChatFinishReason.Stop))
            .Returns(ChatFinishReasonDto.Stop);

        var result = _mapper.ToApplication(domain);

        Assert.Equal(2, result.Messages.Count);
        Assert.Equal(messageDto1, result.Messages[0]);
        Assert.Equal(messageDto2, result.Messages[1]);
        _chatMessageMapperMock.Verify(m => m.ToApplication(It.IsAny<ChatMessage>()), Times.Exactly(2));
    }

    [Fact]
    public void ToDomain_AllPropertiesSet_MapsCorrectly()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var messageDto = new ChatMessageDto(ChatRoleEnumDto.System, [new TextContentDto(null, "System message")]);
        var usageDto = new UsageDetailsDto(15, 25, 40, null, null, null);

        var dto = new ChatResponseDto(
            Messages: [messageDto],
            ResponseId: "resp-abc",
            ConversationId: "conv-def",
            ModelId: "gpt-4-turbo",
            CreatedAt: createdAt,
            FinishReason: ChatFinishReasonDto.ToolCalls,
            Usage: usageDto
        );

        var message = new ChatMessage(ChatRole.System, [new TextContent("System message")]);
        var usage = new UsageDetails
        {
            InputTokenCount = 15,
            OutputTokenCount = 25,
            TotalTokenCount = 40
        };

        _chatMessageMapperMock
            .Setup(m => m.ToDomain(messageDto))
            .Returns(message);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToDomain(ChatFinishReasonDto.ToolCalls))
            .Returns(ChatFinishReason.ToolCalls);
        _usageDetailsMapperMock
            .Setup(m => m.ToDomain(usageDto))
            .Returns(usage);

        var result = _mapper.ToDomain(dto);

        Assert.Single(result.Messages);
        Assert.Equal(message, result.Messages[0]);
        Assert.Equal("resp-abc", result.ResponseId);
        Assert.Equal("conv-def", result.ConversationId);
        Assert.Equal("gpt-4-turbo", result.ModelId);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal(ChatFinishReason.ToolCalls, result.FinishReason);
        Assert.Equal(usage, result.Usage);
        _chatMessageMapperMock.Verify(m => m.ToDomain(messageDto), Times.Once);
        _chatFinishReasonMapperMock.Verify(m => m.ToDomain(ChatFinishReasonDto.ToolCalls), Times.Once);
        _usageDetailsMapperMock.Verify(m => m.ToDomain(usageDto), Times.Once);
    }

    [Fact]
    public void ToDomain_UsageNull_MapsCorrectly()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var messageDto = new ChatMessageDto(ChatRoleEnumDto.Assistant, [new TextContentDto(null, "Response")]);

        var dto = new ChatResponseDto(
            Messages: [messageDto],
            ResponseId: "resp-xyz",
            ConversationId: "conv-uvw",
            ModelId: "claude-3",
            CreatedAt: createdAt,
            FinishReason: ChatFinishReasonDto.ContentFilter,
            Usage: null
        );

        var message = new ChatMessage(ChatRole.Assistant, [new TextContent("Response")]);

        _chatMessageMapperMock
            .Setup(m => m.ToDomain(messageDto))
            .Returns(message);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToDomain(ChatFinishReasonDto.ContentFilter))
            .Returns(ChatFinishReason.ContentFilter);

        var result = _mapper.ToDomain(dto);

        Assert.Null(result.Usage);
        Assert.Equal(ChatFinishReason.ContentFilter, result.FinishReason);
        _usageDetailsMapperMock.Verify(m => m.ToDomain(It.IsAny<UsageDetailsDto>()), Times.Never);
    }

    [Fact]
    public void ToDomain_MultipleMessages_MapsCorrectly()
    {
        var messageDto1 = new ChatMessageDto(ChatRoleEnumDto.User, [new TextContentDto(null, "First")]);
        var messageDto2 = new ChatMessageDto(ChatRoleEnumDto.Assistant, [new TextContentDto(null, "Second")]);
        var messageDto3 = new ChatMessageDto(ChatRoleEnumDto.User, [new TextContentDto(null, "Third")]);

        var dto = new ChatResponseDto(
            Messages: [messageDto1, messageDto2, messageDto3],
            ResponseId: "resp-multi",
            ConversationId: "conv-multi",
            ModelId: "gpt-4",
            CreatedAt: DateTimeOffset.UtcNow,
            FinishReason: ChatFinishReasonDto.Stop,
            Usage: null
        );

        var message1 = new ChatMessage(ChatRole.User, [new TextContent("First")]);
        var message2 = new ChatMessage(ChatRole.Assistant, [new TextContent("Second")]);
        var message3 = new ChatMessage(ChatRole.User, [new TextContent("Third")]);

        _chatMessageMapperMock.Setup(m => m.ToDomain(messageDto1)).Returns(message1);
        _chatMessageMapperMock.Setup(m => m.ToDomain(messageDto2)).Returns(message2);
        _chatMessageMapperMock.Setup(m => m.ToDomain(messageDto3)).Returns(message3);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToDomain(ChatFinishReasonDto.Stop))
            .Returns(ChatFinishReason.Stop);

        var result = _mapper.ToDomain(dto);

        Assert.Equal(3, result.Messages.Count);
        Assert.Equal(message1, result.Messages[0]);
        Assert.Equal(message2, result.Messages[1]);
        Assert.Equal(message3, result.Messages[2]);
        _chatMessageMapperMock.Verify(m => m.ToDomain(It.IsAny<ChatMessageDto>()), Times.Exactly(3));
    }
}
