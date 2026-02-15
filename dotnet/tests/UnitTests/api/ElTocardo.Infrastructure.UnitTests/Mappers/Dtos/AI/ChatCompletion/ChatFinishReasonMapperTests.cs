using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ChatFinishReasonMapperTests
{

    private static readonly Mock<ILogger<ChatFinishReasonMapper>> LoggerMock = new();
    private readonly IDomainEntityMapper<ChatFinishReason, ChatFinishReasonDto> _mapper = new ChatFinishReasonMapper(LoggerMock.Object);

    [Theory]
    [InlineData("stop", ChatFinishReasonDto.Stop)]
    [InlineData("length", ChatFinishReasonDto.Length)]
    [InlineData("tool_calls", ChatFinishReasonDto.ToolCalls)]
    [InlineData("content_filter", ChatFinishReasonDto.ContentFilter)]
    public void ToApplication_KnownValue_MapsCorrectly(string domain, ChatFinishReasonDto expectedDto)
    {
        var result = _mapper.ToApplication(SimulateEnum(domain));

        Assert.Equal(expectedDto, result);
    }


    public ChatFinishReason SimulateEnum(string enumValue)
    {
        switch (enumValue)
        {
            case "stop":
                return ChatFinishReason.Stop;
            case "length":
                return ChatFinishReason.Length;
            case "tool_calls":
                return ChatFinishReason.ToolCalls;
            case "content_filter":
                return ChatFinishReason.ContentFilter;
            default:
                return ChatFinishReason.Stop;
        }
    }



    [Theory]
    [InlineData(ChatFinishReasonDto.Stop, "stop")]
    [InlineData(ChatFinishReasonDto.Length, "length")]
    [InlineData(ChatFinishReasonDto.ToolCalls, "tool_calls")]
    [InlineData(ChatFinishReasonDto.ContentFilter, "content_filter")]
    public void ToDomain_KnownValue_MapsCorrectly(ChatFinishReasonDto dto, string expectedDomain)
    {
        var result = _mapper.ToDomain(dto);

        Assert.Equal(SimulateEnum(expectedDomain), result);
    }

    [Fact]
    public void ToApplication_UnknownValue_ThrowsNotSupportedException()
    {
        var unknown = new ChatFinishReason("unknown");

        Assert.Throws<NotSupportedException>(() => _mapper.ToApplication(unknown));
    }

    [Fact]
    public void ToDomain_UnknownValue_ThrowsNotSupportedException()
    {
        var unknown = (ChatFinishReasonDto)999;

        Assert.Throws<NotSupportedException>(() => _mapper.ToDomain(unknown));
    }
}
