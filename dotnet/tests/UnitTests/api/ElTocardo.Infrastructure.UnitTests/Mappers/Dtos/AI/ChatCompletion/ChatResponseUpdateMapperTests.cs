using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ChatResponseUpdateMapperTests
{
    private readonly Mock<IDomainEntityMapper<ChatFinishReason, ChatFinishReasonDto>> _chatFinishReasonMapperMock;
    private readonly Mock<IDomainEntityMapper<ChatRole, ChatRoleEnumDto>> _chatRoleMapperMock;
    private readonly Mock<IDomainEntityMapper<AIContent, AiContentDto>> _aiContentMapperMock;
    private readonly IDomainEntityMapper<ChatResponseUpdate, ChatResponseUpdateDto> _mapper;

    public ChatResponseUpdateMapperTests()
    {
        _chatFinishReasonMapperMock = new Mock<IDomainEntityMapper<ChatFinishReason, ChatFinishReasonDto>>();
        _chatRoleMapperMock = new Mock<IDomainEntityMapper<ChatRole, ChatRoleEnumDto>>();
        _aiContentMapperMock = new Mock<IDomainEntityMapper<AIContent, AiContentDto>>();

        _mapper = new ChatResponseUpdateMapper(
            _chatFinishReasonMapperMock.Object,
            _chatRoleMapperMock.Object,
            _aiContentMapperMock.Object
        );
    }

    [Fact]
    public void ToApplication_AllPropertiesSet_MapsCorrectly()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var content = new TextContent("Streaming response");

        var domain = new ChatResponseUpdate
        {
            AuthorName = "Assistant",
            Role = ChatRole.Assistant,
            Contents = [content],
            ResponseId = "resp-stream-123",
            MessageId = "msg-456",
            ConversationId = "conv-789",
            CreatedAt = createdAt,
            FinishReason = ChatFinishReason.Stop,
            ModelId = "gpt-4"
        };

        var contentDto = new TextContentDto(null, "Streaming response");

        _chatRoleMapperMock
            .Setup(m => m.ToApplication(ChatRole.Assistant))
            .Returns(ChatRoleEnumDto.Assistant);
        _aiContentMapperMock
            .Setup(m => m.ToApplication(content))
            .Returns(contentDto);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToApplication(ChatFinishReason.Stop))
            .Returns(ChatFinishReasonDto.Stop);

        var result = _mapper.ToApplication(domain);

        Assert.Equal("Assistant", result.AuthorName);
        Assert.Equal(ChatRoleEnumDto.Assistant, result.Role);
        Assert.Single(result.Contents);
        Assert.Equal(contentDto, result.Contents[0]);
        Assert.Equal("resp-stream-123", result.ResponseId);
        Assert.Equal("msg-456", result.MessageId);
        Assert.Equal("conv-789", result.ConversationId);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal(ChatFinishReasonDto.Stop, result.FinishReason);
        Assert.Equal("gpt-4", result.ModelId);
        _chatRoleMapperMock.Verify(m => m.ToApplication(ChatRole.Assistant), Times.Once);
        _aiContentMapperMock.Verify(m => m.ToApplication(content), Times.Once);
        _chatFinishReasonMapperMock.Verify(m => m.ToApplication(ChatFinishReason.Stop), Times.Once);
    }

    [Fact]
    public void ToApplication_NullablePropertiesNull_MapsCorrectly()
    {
        var content = new TextContent("Content");

        var domain = new ChatResponseUpdate
        {
            AuthorName = null,
            Role = ChatRole.User,
            Contents = [content],
            ResponseId = null,
            MessageId = null,
            ConversationId = null,
            CreatedAt = null,
            FinishReason = ChatFinishReason.Length,
            ModelId = null
        };

        var contentDto = new TextContentDto(null, "Content");

        _chatRoleMapperMock
            .Setup(m => m.ToApplication(ChatRole.User))
            .Returns(ChatRoleEnumDto.User);
        _aiContentMapperMock
            .Setup(m => m.ToApplication(content))
            .Returns(contentDto);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToApplication(ChatFinishReason.Length))
            .Returns(ChatFinishReasonDto.Length);

        var result = _mapper.ToApplication(domain);

        Assert.Null(result.AuthorName);
        Assert.Equal(ChatRoleEnumDto.User, result.Role);
        Assert.Null(result.ResponseId);
        Assert.Null(result.MessageId);
        Assert.Null(result.ConversationId);
        Assert.Null(result.CreatedAt);
        Assert.Null(result.ModelId);
        Assert.Equal(ChatFinishReasonDto.Length, result.FinishReason);
    }

    [Fact]
    public void ToApplication_MultipleContents_MapsCorrectly()
    {
        var content1 = new TextContent("Part 1");
        var content2 = new TextContent("Part 2");
        var content3 = new TextContent("Part 3");

        var domain = new ChatResponseUpdate
        {
            AuthorName = "AI",
            Role = ChatRole.Assistant,
            Contents = [content1, content2, content3],
            ResponseId = "resp-multi",
            MessageId = "msg-multi",
            ConversationId = "conv-multi",
            CreatedAt = DateTimeOffset.UtcNow,
            FinishReason = ChatFinishReason.Stop,
            ModelId = "claude-3"
        };

        var contentDto1 = new TextContentDto(null, "Part 1");
        var contentDto2 = new TextContentDto(null, "Part 2");
        var contentDto3 = new TextContentDto(null, "Part 3");

        _chatRoleMapperMock
            .Setup(m => m.ToApplication(ChatRole.Assistant))
            .Returns(ChatRoleEnumDto.Assistant);
        _aiContentMapperMock.Setup(m => m.ToApplication(content1)).Returns(contentDto1);
        _aiContentMapperMock.Setup(m => m.ToApplication(content2)).Returns(contentDto2);
        _aiContentMapperMock.Setup(m => m.ToApplication(content3)).Returns(contentDto3);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToApplication(ChatFinishReason.Stop))
            .Returns(ChatFinishReasonDto.Stop);

        var result = _mapper.ToApplication(domain);

        Assert.Equal(3, result.Contents.Count);
        Assert.Equal(contentDto1, result.Contents[0]);
        Assert.Equal(contentDto2, result.Contents[1]);
        Assert.Equal(contentDto3, result.Contents[2]);
        _aiContentMapperMock.Verify(m => m.ToApplication(It.IsAny<AIContent>()), Times.Exactly(3));
    }

    [Fact]
    public void ToDomain_AllPropertiesSet_MapsCorrectly()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var contentDto = new TextContentDto(null, "Response update");

        var dto = new ChatResponseUpdateDto(
            AuthorName: "Bot",
            Role: ChatRoleEnumDto.Assistant,
            Contents: [contentDto],
            ResponseId: "resp-abc",
            MessageId: "msg-def",
            ConversationId: "conv-ghi",
            CreatedAt: createdAt,
            FinishReason: ChatFinishReasonDto.ToolCalls,
            ModelId: "gpt-4-turbo"
        );

        var content = new TextContent("Response update");

        _chatRoleMapperMock
            .Setup(m => m.ToDomain(ChatRoleEnumDto.Assistant))
            .Returns(ChatRole.Assistant);
        _aiContentMapperMock
            .Setup(m => m.ToDomain(contentDto))
            .Returns(content);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToDomain(ChatFinishReasonDto.ToolCalls))
            .Returns(ChatFinishReason.ToolCalls);

        var result = _mapper.ToDomain(dto);

        Assert.Equal("Bot", result.AuthorName);
        Assert.Equal(ChatRole.Assistant, result.Role);
        Assert.Single(result.Contents);
        Assert.Equal(content, result.Contents[0]);
        Assert.Equal("resp-abc", result.ResponseId);
        Assert.Equal("msg-def", result.MessageId);
        Assert.Equal("conv-ghi", result.ConversationId);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal(ChatFinishReason.ToolCalls, result.FinishReason);
        Assert.Equal("gpt-4-turbo", result.ModelId);
        _chatRoleMapperMock.Verify(m => m.ToDomain(ChatRoleEnumDto.Assistant), Times.Once);
        _aiContentMapperMock.Verify(m => m.ToDomain(contentDto), Times.Once);
        _chatFinishReasonMapperMock.Verify(m => m.ToDomain(ChatFinishReasonDto.ToolCalls), Times.Once);
    }

    [Fact]
    public void ToDomain_NullablePropertiesNull_MapsCorrectly()
    {
        var contentDto = new TextContentDto(null, "Update");

        var dto = new ChatResponseUpdateDto(
            AuthorName: null,
            Role: ChatRoleEnumDto.System,
            Contents: [contentDto],
            ResponseId: null,
            MessageId: null,
            ConversationId: null,
            CreatedAt: null,
            FinishReason: ChatFinishReasonDto.ContentFilter,
            ModelId: null
        );

        var content = new TextContent("Update");

        _chatRoleMapperMock
            .Setup(m => m.ToDomain(ChatRoleEnumDto.System))
            .Returns(ChatRole.System);
        _aiContentMapperMock
            .Setup(m => m.ToDomain(contentDto))
            .Returns(content);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToDomain(ChatFinishReasonDto.ContentFilter))
            .Returns(ChatFinishReason.ContentFilter);

        var result = _mapper.ToDomain(dto);

        Assert.Null(result.AuthorName);
        Assert.Equal(ChatRole.System, result.Role);
        Assert.Null(result.ResponseId);
        Assert.Null(result.MessageId);
        Assert.Null(result.ConversationId);
        Assert.Null(result.CreatedAt);
        Assert.Null(result.ModelId);
        Assert.Equal(ChatFinishReason.ContentFilter, result.FinishReason);
    }

    [Fact]
    public void ToDomain_MultipleContents_MapsCorrectly()
    {
        var contentDto1 = new TextContentDto(null, "Chunk 1");
        var contentDto2 = new TextContentDto(null, "Chunk 2");

        var dto = new ChatResponseUpdateDto(
            AuthorName: "Streamer",
            Role: ChatRoleEnumDto.Assistant,
            Contents: [contentDto1, contentDto2],
            ResponseId: "resp-stream",
            MessageId: "msg-stream",
            ConversationId: "conv-stream",
            CreatedAt: DateTimeOffset.UtcNow,
            FinishReason: ChatFinishReasonDto.Stop,
            ModelId: "gpt-3.5"
        );

        var content1 = new TextContent("Chunk 1");
        var content2 = new TextContent("Chunk 2");

        _chatRoleMapperMock
            .Setup(m => m.ToDomain(ChatRoleEnumDto.Assistant))
            .Returns(ChatRole.Assistant);
        _aiContentMapperMock.Setup(m => m.ToDomain(contentDto1)).Returns(content1);
        _aiContentMapperMock.Setup(m => m.ToDomain(contentDto2)).Returns(content2);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToDomain(ChatFinishReasonDto.Stop))
            .Returns(ChatFinishReason.Stop);

        var result = _mapper.ToDomain(dto);

        Assert.Equal(2, result.Contents.Count);
        Assert.Equal(content1, result.Contents[0]);
        Assert.Equal(content2, result.Contents[1]);
        _aiContentMapperMock.Verify(m => m.ToDomain(It.IsAny<AiContentDto>()), Times.Exactly(2));
    }

    [Fact]
    public void ToApplication_EmptyContents_MapsCorrectly()
    {
        var domain = new ChatResponseUpdate
        {
            AuthorName = "Test",
            Role = ChatRole.Assistant,
            Contents = [],
            ResponseId = "resp-empty",
            MessageId = "msg-empty",
            ConversationId = "conv-empty",
            CreatedAt = DateTimeOffset.UtcNow,
            FinishReason = ChatFinishReason.Stop,
            ModelId = "test-model"
        };

        _chatRoleMapperMock
            .Setup(m => m.ToApplication(ChatRole.Assistant))
            .Returns(ChatRoleEnumDto.Assistant);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToApplication(ChatFinishReason.Stop))
            .Returns(ChatFinishReasonDto.Stop);

        var result = _mapper.ToApplication(domain);

        Assert.Empty(result.Contents);
        _aiContentMapperMock.Verify(m => m.ToApplication(It.IsAny<AIContent>()), Times.Never);
    }

    [Fact]
    public void ToDomain_EmptyContents_MapsCorrectly()
    {
        var dto = new ChatResponseUpdateDto(
            AuthorName: "Test",
            Role: ChatRoleEnumDto.User,
            Contents: [],
            ResponseId: "resp-empty",
            MessageId: "msg-empty",
            ConversationId: "conv-empty",
            CreatedAt: DateTimeOffset.UtcNow,
            FinishReason: ChatFinishReasonDto.Stop,
            ModelId: "test-model"
        );

        _chatRoleMapperMock
            .Setup(m => m.ToDomain(ChatRoleEnumDto.User))
            .Returns(ChatRole.User);
        _chatFinishReasonMapperMock
            .Setup(m => m.ToDomain(ChatFinishReasonDto.Stop))
            .Returns(ChatFinishReason.Stop);

        var result = _mapper.ToDomain(dto);

        Assert.Empty(result.Contents);
        _aiContentMapperMock.Verify(m => m.ToDomain(It.IsAny<AiContentDto>()), Times.Never);
    }
}
