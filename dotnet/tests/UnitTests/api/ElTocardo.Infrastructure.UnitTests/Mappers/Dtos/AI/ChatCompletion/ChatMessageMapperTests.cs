using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ChatMessageMapperTests
{
    private readonly Mock<IDomainEntityMapper<ChatRole, ChatRoleEnumDto>> _chatRoleMapperMock;
    private readonly Mock<IDomainEntityMapper<AIContent, AiContentDto>> _aiContentMapperMock;
    private readonly IDomainEntityMapper<ChatMessage, ChatMessageDto> _mapper;

    public ChatMessageMapperTests()
    {
        _chatRoleMapperMock = new Mock<IDomainEntityMapper<ChatRole, ChatRoleEnumDto>>();
        _aiContentMapperMock = new Mock<IDomainEntityMapper<AIContent, AiContentDto>>();
        _mapper = new ChatMessageMapper(_chatRoleMapperMock.Object, _aiContentMapperMock.Object);
    }

    [Fact]
    public void ToApplication_ValidChatMessage_MapsCorrectly()
    {
        var textContent = new TextContent("Hello");
        var domain = new ChatMessage(ChatRole.User, [textContent]);
        var textContentDto = new TextContentDto(null, "Hello");

        _chatRoleMapperMock
            .Setup(m => m.ToApplication(ChatRole.User))
            .Returns(ChatRoleEnumDto.User);
        _aiContentMapperMock
            .Setup(m => m.ToApplication(textContent))
            .Returns(textContentDto);

        var result = _mapper.ToApplication(domain);

        Assert.Equal(ChatRoleEnumDto.User, result.Role);
        Assert.Single(result.Contents);
        Assert.IsType<TextContentDto>(result.Contents[0]);
        _chatRoleMapperMock.Verify(m => m.ToApplication(ChatRole.User), Times.Once);
        _aiContentMapperMock.Verify(m => m.ToApplication(textContent), Times.Once);
    }

    [Fact]
    public void ToApplication_MessageWithMultipleContents_MapsCorrectly()
    {
        var textContent1 = new TextContent("First");
        var textContent2 = new TextContent("Second");
        var domain = new ChatMessage(ChatRole.Assistant, [textContent1, textContent2]);
        var textContentDto1 = new TextContentDto(null, "First");
        var textContentDto2 = new TextContentDto(null, "Second");

        _chatRoleMapperMock
            .Setup(m => m.ToApplication(ChatRole.Assistant))
            .Returns(ChatRoleEnumDto.Assistant);
        _aiContentMapperMock
            .Setup(m => m.ToApplication(textContent1))
            .Returns(textContentDto1);
        _aiContentMapperMock
            .Setup(m => m.ToApplication(textContent2))
            .Returns(textContentDto2);

        var result = _mapper.ToApplication(domain);

        Assert.Equal(ChatRoleEnumDto.Assistant, result.Role);
        Assert.Equal(2, result.Contents.Count);
        _aiContentMapperMock.Verify(m => m.ToApplication(It.IsAny<AIContent>()), Times.Exactly(2));
    }

    [Fact]
    public void ToDomain_ValidChatMessageDto_MapsCorrectly()
    {
        var textContentDto = new TextContentDto(null, "Hello");
        var dto = new ChatMessageDto(ChatRoleEnumDto.System, [textContentDto]);
        var textContent = new TextContent("Hello");

        _chatRoleMapperMock
            .Setup(m => m.ToDomain(ChatRoleEnumDto.System))
            .Returns(ChatRole.System);
        _aiContentMapperMock
            .Setup(m => m.ToDomain(textContentDto))
            .Returns(textContent);

        var result = _mapper.ToDomain(dto);

        Assert.Equal(ChatRole.System, result.Role);
        Assert.Single(result.Contents);
        Assert.IsType<TextContent>(result.Contents[0]);
        _chatRoleMapperMock.Verify(m => m.ToDomain(ChatRoleEnumDto.System), Times.Once);
        _aiContentMapperMock.Verify(m => m.ToDomain(textContentDto), Times.Once);
    }

    [Fact]
    public void ToDomain_MessageDtoWithMultipleContents_MapsCorrectly()
    {
        var textContentDto1 = new TextContentDto(null, "First");
        var textContentDto2 = new TextContentDto(null, "Second");
        var dto = new ChatMessageDto(ChatRoleEnumDto.Tool, [textContentDto1, textContentDto2]);
        var textContent1 = new TextContent("First");
        var textContent2 = new TextContent("Second");

        _chatRoleMapperMock
            .Setup(m => m.ToDomain(ChatRoleEnumDto.Tool))
            .Returns(ChatRole.Tool);
        _aiContentMapperMock
            .Setup(m => m.ToDomain(textContentDto1))
            .Returns(textContent1);
        _aiContentMapperMock
            .Setup(m => m.ToDomain(textContentDto2))
            .Returns(textContent2);

        var result = _mapper.ToDomain(dto);

        Assert.Equal(ChatRole.Tool, result.Role);
        Assert.Equal(2, result.Contents.Count);
        _aiContentMapperMock.Verify(m => m.ToDomain(It.IsAny<AiContentDto>()), Times.Exactly(2));
    }
}
