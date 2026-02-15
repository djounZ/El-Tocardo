using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ChatToolModeMapperTests
{
    private static readonly Mock<ILogger<ChatToolModeMapper>> LoggerMock = new();
    private readonly IDomainEntityMapper<ChatToolMode, ChatToolModeDto> _mapper = new ChatToolModeMapper(LoggerMock.Object);

    [Fact]
    public void ToApplication_AutoChatToolMode_MapsCorrectly()
    {
        var domain = new AutoChatToolMode();

        var result = _mapper.ToApplication(domain);

        Assert.IsType<AutoChatToolModeDto>(result);
    }

    [Fact]
    public void ToApplication_NoneChatToolMode_MapsCorrectly()
    {
        var domain = new NoneChatToolMode();

        var result = _mapper.ToApplication(domain);

        Assert.IsType<NoneChatToolModeDto>(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("testFunction")]
    public void ToApplication_RequiredChatToolMode_MapsCorrectly(string? functionName)
    {
        var domain = new RequiredChatToolMode(functionName);

        var result = _mapper.ToApplication(domain);

        Assert.IsType<RequiredChatToolModeDto>(result);
        var dto = (RequiredChatToolModeDto)result;
        Assert.Equal(functionName, dto.RequiredFunctionName);
    }

    [Fact]
    public void ToDomain_AutoChatToolModeDto_MapsCorrectly()
    {
        var dto = new AutoChatToolModeDto();

        var result = _mapper.ToDomain(dto);

        Assert.IsType<AutoChatToolMode>(result);
    }

    [Fact]
    public void ToDomain_NoneChatToolModeDto_MapsCorrectly()
    {
        var dto = new NoneChatToolModeDto();

        var result = _mapper.ToDomain(dto);

        Assert.IsType<NoneChatToolMode>(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("testFunction")]
    public void ToDomain_RequiredChatToolModeDto_MapsCorrectly(string? functionName)
    {
        var dto = new RequiredChatToolModeDto(functionName);

        var result = _mapper.ToDomain(dto);

        Assert.IsType<RequiredChatToolMode>(result);
        var domain = (RequiredChatToolMode)result;
        Assert.Equal(functionName, domain.RequiredFunctionName);
    }
}
