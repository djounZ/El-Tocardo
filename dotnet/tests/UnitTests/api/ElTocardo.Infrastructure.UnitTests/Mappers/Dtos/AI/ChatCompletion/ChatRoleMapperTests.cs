using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ChatRoleMapperTests
{
    private static readonly Mock<ILogger<ChatRoleMapper>> LoggerMock = new();
    private readonly IDomainEntityMapper<ChatRole, ChatRoleEnumDto> _mapper = new ChatRoleMapper(LoggerMock.Object);

    [Theory]
    [InlineData("system", ChatRoleEnumDto.System)]
    [InlineData("assistant", ChatRoleEnumDto.Assistant)]
    [InlineData("user", ChatRoleEnumDto.User)]
    [InlineData("tool", ChatRoleEnumDto.Tool)]
    public void ToApplication_KnownValue_MapsCorrectly(string roleValue, ChatRoleEnumDto expectedDto)
    {
        var domain = new ChatRole(roleValue);

        var result = _mapper.ToApplication(domain);

        Assert.Equal(expectedDto, result);
    }

    [Theory]
    [InlineData(ChatRoleEnumDto.System, "system")]
    [InlineData(ChatRoleEnumDto.Assistant, "assistant")]
    [InlineData(ChatRoleEnumDto.User, "user")]
    [InlineData(ChatRoleEnumDto.Tool, "tool")]
    public void ToDomain_KnownValue_MapsCorrectly(ChatRoleEnumDto dto, string expectedRoleValue)
    {
        var result = _mapper.ToDomain(dto);

        Assert.Equal(new ChatRole(expectedRoleValue), result);
    }

    [Fact]
    public void ToApplication_UnknownValue_ThrowsArgumentOutOfRangeException()
    {
        var unknown = new ChatRole("unknown");

        Assert.Throws<ArgumentOutOfRangeException>(() => _mapper.ToApplication(unknown));
    }

    [Fact]
    public void ToDomain_UnknownValue_ThrowsArgumentOutOfRangeException()
    {
        var unknown = (ChatRoleEnumDto)999;

        Assert.Throws<ArgumentOutOfRangeException>(() => _mapper.ToDomain(unknown));
    }
}
