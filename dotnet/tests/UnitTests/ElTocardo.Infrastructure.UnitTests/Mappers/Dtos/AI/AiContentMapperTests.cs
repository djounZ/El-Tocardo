using System.Diagnostics;
using ElTocardo.Application.Dtos.AI.Contents;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI;

public class AiContentMapperTests
{
    private readonly Mock<ILogger<AiContentMapper>> _loggerMock = new();
    private readonly AiContentMapper _mapper;

    public AiContentMapperTests()
    {
        _mapper = new AiContentMapper(_loggerMock.Object);
    }

    [Fact]
    public void MapToAiContent_DataContentDto_MapsCorrectly()
    {
        // Arrange
        var dto = new DataContentDto(
            null,
            new Uri(
                "data:image/png;base64,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"),
            "image/png",
            "file.png");

        // Act
        var result = _mapper.MapToAiContent(dto);

        // Assert
        result.Should().BeOfType<DataContent>();
        var dataContent = (DataContent)result;
        dataContent.Uri.Should().Be(dto.Uri.ToString());
        dataContent.MediaType.Should().Be(dto.MediaType);
    }

    [Fact]
    public void MapToAiContent_ErrorContentDto_MapsCorrectly()
    {
        var dto = new ErrorContentDto(
            null,
            "error message");
        var result = _mapper.MapToAiContent(dto);
        result.Should().BeOfType<ErrorContent>();
        var errorContent = (ErrorContent)result;
        errorContent.Message.Should().Be(dto.Message);
    }

    [Fact]
    public void MapToAiContent_FunctionCallContentDto_MapsCorrectly()
    {
        var dto = new FunctionCallContentDto(
            null,
            "call-1",
            "func",
            new Dictionary<string, object?> { { "arg1", 42 } });
        var result = _mapper.MapToAiContent(dto);
        result.Should().BeOfType<FunctionCallContent>();
        var callContent = (FunctionCallContent)result;
        callContent.CallId.Should().Be(dto.CallId);
        callContent.Name.Should().Be(dto.Name);
        callContent.Arguments.Should().BeEquivalentTo(dto.Arguments);
    }

    [Fact]
    public void MapToAiContent_FunctionResultContentDto_MapsCorrectly()
    {
        var dto = new FunctionResultContentDto(
            null,
            "call-2",
            123);
        var result = _mapper.MapToAiContent(dto);
        result.Should().BeOfType<FunctionResultContent>();
        var resultContent = (FunctionResultContent)result;
        resultContent.CallId.Should().Be(dto.CallId);
        resultContent.Result.Should().Be(dto.Result);
    }

    [Fact]
    public void MapToAiContent_TextContentDto_MapsCorrectly()
    {
        var dto = new TextContentDto(
            null,
            "hello world");
        var result = _mapper.MapToAiContent(dto);
        result.Should().BeOfType<TextContent>();
        var textContent = (TextContent)result;
        textContent.Text.Should().Be(dto.Text);
    }

    [Fact]
    public void MapToAiContent_TextReasoningContentDto_MapsCorrectly()
    {
        var dto = new TextReasoningContentDto(
            null,
            "reasoning");
        var result = _mapper.MapToAiContent(dto);
        result.Should().BeOfType<TextReasoningContent>();
        var reasoningContent = (TextReasoningContent)result;
        reasoningContent.Text.Should().Be(dto.Text);
    }

    [Fact]
    public void MapToAiContent_UriContentDto_MapsCorrectly()
    {
        var dto = new UriContentDto(
            null,
            new Uri("https://example.com/image.png"),
            "image/png");
        var result = _mapper.MapToAiContent(dto);
        result.Should().BeOfType<UriContent>();
        var uriContent = (UriContent)result;
        uriContent.Uri.ToString().Should().Be(dto.Uri.ToString());
        uriContent.MediaType.Should().Be(dto.MediaType);
    }

    [Fact]
    public void MapToAiContent_UsageContentDto_MapsCorrectly()
    {
        var detailsDto = new UsageDetailsDto(1, 2, 3, new Dictionary<string, long> { { "foo", 42 } });
        var dto = new UsageContentDto(
            null,
            detailsDto);
        var result = _mapper.MapToAiContent(dto);
        result.Should().BeOfType<UsageContent>();
        var usageContent = (UsageContent)result;
        usageContent.Details.InputTokenCount.Should().Be(detailsDto.InputTokenCount);
        usageContent.Details.OutputTokenCount.Should().Be(detailsDto.OutputTokenCount);
        usageContent.Details.TotalTokenCount.Should().Be(detailsDto.TotalTokenCount);
        Debug.Assert(usageContent.Details.AdditionalCounts != null);
        usageContent.Details.AdditionalCounts["foo"].Should().Be(42);
    }

    [Fact]
    public void MapToAiContent_UnsupportedDto_Throws()
    {
        var unsupported = new DummyContentDto(null);
        Action act = () => _mapper.MapToAiContent(unsupported);
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void MapToUsageContentDto_MapsCorrectly()
    {
        var details = new UsageDetails
        {
            InputTokenCount = 1,
            OutputTokenCount = 2,
            TotalTokenCount = 3,
            AdditionalCounts = new AdditionalPropertiesDictionary<long> { { "foo", 42 } }
        };
        var usageContent = new UsageContent(details);
        var dto = _mapper.MapToAiContentDto(usageContent) as UsageContentDto;
        dto.Should().NotBeNull();
        dto!.DetailsDto.InputTokenCount.Should().Be(1);
        dto.DetailsDto.OutputTokenCount.Should().Be(2);
        dto.DetailsDto.TotalTokenCount.Should().Be(3);
        dto.DetailsDto.AdditionalCounts!["foo"].Should().Be(42);
    }

    [Fact]
    public void MapToUriContentDto_MapsCorrectly()
    {
        var uriContent = new UriContent(new Uri("https://example.com/image.png"), "image/png");
        var dto = _mapper.MapToAiContentDto(uriContent) as UriContentDto;
        dto.Should().NotBeNull();
        dto!.Uri.Should().Be(uriContent.Uri);
        dto.MediaType.Should().Be(uriContent.MediaType);
    }

    [Fact]
    public void MapToTextReasoningContentDto_MapsCorrectly()
    {
        var textReasoningContent = new TextReasoningContent("reasoning");
        var dto = _mapper.MapToAiContentDto(textReasoningContent) as TextReasoningContentDto;
        dto.Should().NotBeNull();
        dto!.Text.Should().Be(textReasoningContent.Text);
    }

    [Fact]
    public void MapToTextContentDto_MapsCorrectly()
    {
        var textContent = new TextContent("hello");
        var dto = _mapper.MapToAiContentDto(textContent) as TextContentDto;
        dto.Should().NotBeNull();
        dto!.Text.Should().Be(textContent.Text);
    }

    [Fact]
    public void MapToFunctionResultContentDto_MapsCorrectly()
    {
        var functionResultContent = new FunctionResultContent("call-2", 123);
        var dto = _mapper.MapToAiContentDto(functionResultContent) as FunctionResultContentDto;
        dto.Should().NotBeNull();
        dto!.CallId.Should().Be(functionResultContent.CallId);
        dto.Result.Should().Be(functionResultContent.Result);
    }

    [Fact]
    public void MapToFunctionCallContentDto_MapsCorrectly()
    {
        var functionCallContent =
            new FunctionCallContent("call-1", "func", new Dictionary<string, object?> { { "arg1", 42 } });
        var dto = _mapper.MapToAiContentDto(functionCallContent) as FunctionCallContentDto;
        dto.Should().NotBeNull();
        dto!.CallId.Should().Be(functionCallContent.CallId);
        dto.Name.Should().Be(functionCallContent.Name);
        dto.Arguments.Should().BeEquivalentTo(functionCallContent.Arguments);
    }

    [Fact]
    public void MapToErrorContentDto_MapsCorrectly()
    {
        var errorContent = new ErrorContent("error message");
        var dto = _mapper.MapToAiContentDto(errorContent) as ErrorContentDto;
        dto.Should().NotBeNull();
        dto!.Message.Should().Be(errorContent.Message);
    }

    [Fact]
    public void MapToDataContentDto_MapsCorrectly()
    {
        var dataContent = new DataContent(
            "data:image/png;base64,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",
            "image/png");
        var dto = _mapper.MapToAiContentDto(dataContent) as DataContentDto;
        dto.Should().NotBeNull();
        dto!.Uri.ToString().Should().Be(dataContent.Uri);
        dto.MediaType.Should().Be(dataContent.MediaType);
    }

    internal sealed record DummyContentDto(IList<AiAnnotationDto>? Annotations) : AiContentDto(Annotations);
}
