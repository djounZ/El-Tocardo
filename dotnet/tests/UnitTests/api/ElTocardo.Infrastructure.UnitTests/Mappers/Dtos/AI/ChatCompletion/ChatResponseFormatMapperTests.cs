using System.Text.Json;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ChatResponseFormatMapperTests
{
    private static readonly Mock<ILogger<ChatResponseFormatMapper>> LoggerMock = new();
    private readonly IDomainEntityMapper<ChatResponseFormat, ChatResponseFormatDto> _mapper = new ChatResponseFormatMapper(LoggerMock.Object);

    [Fact]
    public void ToApplication_ChatResponseFormatText_MapsCorrectly()
    {
        var domain = new ChatResponseFormatText();

        var result = _mapper.ToApplication(domain);

        Assert.IsType<ChatResponseFormatTextDto>(result);
    }

    [Fact]
    public void ToApplication_ChatResponseFormatJson_MapsCorrectly()
    {
        var schema = JsonDocument.Parse("{\"type\":\"object\"}").RootElement;
        var domain = new ChatResponseFormatJson(schema, "TestSchema", "Test Description");

        var result = _mapper.ToApplication(domain);

        Assert.IsType<ChatResponseFormatJsonDto>(result);
        var dto = (ChatResponseFormatJsonDto)result;
        Assert.Contains("object", dto.Schema);
        Assert.Equal("TestSchema", dto.SchemaName);
        Assert.Equal("Test Description", dto.SchemaDescription);
    }

    [Fact]
    public void ToApplication_ChatResponseFormatJsonWithNullValues_MapsCorrectly()
    {
        var schema = JsonDocument.Parse("{}").RootElement;
        var domain = new ChatResponseFormatJson(schema, null, null);

        var result = _mapper.ToApplication(domain);

        Assert.IsType<ChatResponseFormatJsonDto>(result);
        var dto = (ChatResponseFormatJsonDto)result;
        Assert.NotNull(dto.Schema);
        Assert.Null(dto.SchemaName);
        Assert.Null(dto.SchemaDescription);
    }

    [Fact]
    public void ToDomain_ChatResponseFormatTextDto_MapsCorrectly()
    {
        var dto = new ChatResponseFormatTextDto();

        var result = _mapper.ToDomain(dto);

        Assert.IsType<ChatResponseFormatText>(result);
    }

    [Fact]
    public void ToDomain_ChatResponseFormatJsonDto_MapsCorrectly()
    {
        var dto = new ChatResponseFormatJsonDto("{\"type\":\"object\"}", "TestSchema", "Test Description");

        var result = _mapper.ToDomain(dto);

        Assert.IsType<ChatResponseFormatJson>(result);
        var domain = (ChatResponseFormatJson)result;
        Assert.NotNull(domain.Schema);
        Assert.Equal("TestSchema", domain.SchemaName);
        Assert.Equal("Test Description", domain.SchemaDescription);
    }

    [Fact]
    public void ToDomain_ChatResponseFormatJsonDtoWithNullSchema_MapsCorrectly()
    {
        var dto = new ChatResponseFormatJsonDto(null, null, null);

        var result = _mapper.ToDomain(dto);

        Assert.IsType<ChatResponseFormatJson>(result);
    }
}
