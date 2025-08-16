using System.Reflection;
using System.Text.Json;
using AI.GithubCopilot.Infrastructure.Dtos.ChatCompletion;
using AI.GithubCopilot.Infrastructure.Mappers.Dtos;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace AI.GithubCopilot.UnitTests.Infrastructure.Mappers.Dtos;

public class ChatCompletionMapperTests
{
    private readonly Mock<ILogger<ChatCompletionMapper>> _loggerMock = new();
    private readonly ChatCompletionMapper _mapper;

    public ChatCompletionMapperTests()
    {
        _mapper = new ChatCompletionMapper(_loggerMock.Object);
    }

    [Fact]
    public void MapToChatCompletionRequestDto_MapsCorrectly()
    {
        // Arrange
        var messages = new[] { new ChatMessage(ChatRole.User, new List<AIContent> { new TextContent("Hello") }) };
        var options = new ChatOptions
        {
            ModelId = "gpt-4",
            Temperature = 0.7f,
            MaxOutputTokens = 128,
            TopP = 0.9f,
            FrequencyPenalty = 0.1f,
            PresencePenalty = 0.2f,
            StopSequences = new List<string> { "stop" },
            Seed = 42,
            AllowMultipleToolCalls = true
        };
        var domainModel = new ChatCompletionMapper.ChatCompletionRequest(messages, options);

        // Act
        var dto = _mapper.MapToChatCompletionRequestDto(domainModel);

        // Assert
        dto.Messages.Should().HaveCount(1);
        dto.Model.Should().Be("gpt-4");
        dto.Temperature.Should().Be(0.7f);
        dto.MaxTokens.Should().Be(128);
        dto.TopP.Should().Be(0.9f);
        dto.FrequencyPenalty.Should().Be(0.1f);
        dto.PresencePenalty.Should().Be(0.2f);
        dto.Stop.Should().ContainSingle(s => s == "stop");
        dto.Seed.Should().Be(42);
        dto.ParallelToolCalls.Should().BeTrue();
    }

    [Fact]
    public void MapToChatCompletionRequest_MapsCorrectly()
    {
        // Arrange
        var chatMessageDtos = new[]
        {
            new ChatMessageDto(
                "user",
                new TextContentDto("Hello"),
                "user1")
        };
        var dto = new ChatCompletionRequestDto(
            chatMessageDtos,
            "gpt-4",
            0.7,
            128,
            false,
            0.9,
            0.1,
            0.2,
            ["stop"],
            null,
            null,
            null,
            null,
            null,
            null,
            42,
            null,
            null,
            true
        );

        // Act
        var result = _mapper.MapToChatCompletionRequest(dto);

        // Assert
        result.Messages.Should().HaveCount(1);
        result.Options.Should().NotBeNull();
        result.Options!.ModelId.Should().Be("gpt-4");
        result.Options.Temperature.Should().Be(0.7f);
        result.Options.MaxOutputTokens.Should().Be(128);
        result.Options.TopP.Should().Be(0.9f);
        result.Options.FrequencyPenalty.Should().Be(0.1f);
        result.Options.PresencePenalty.Should().Be(0.2f);
        result.Options.StopSequences.Should().ContainSingle(s => s == "stop");
        result.Options.Seed.Should().Be(42);
        result.Options.AllowMultipleToolCalls.Should().BeTrue();
    }

    [Fact]
    public void MapToChatResponse_MapsCorrectly()
    {
        // Arrange
        var responseDto = new ChatCompletionResponseDto(
            [
                new ChatChoiceDto(
                    0,
                    new ChatMessageDto("assistant", new TextContentDto("Hi!")),
                    null,
                    "stop")
            ],
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            "resp-1",
            "chat.completion",
            "gpt-4",
            null,
            new UsageDto(1, 2, 3)
        );

        // Act
        var result = _mapper.MapToChatResponse(responseDto);

        // Assert
        result.Should().NotBeNull();
        result.ResponseId.Should().Be("resp-1");
        result.ModelId.Should().Be("gpt-4");
        result.Usage.Should().NotBeNull();
        result.Usage!.InputTokenCount.Should().Be(2);
        result.Usage.OutputTokenCount.Should().Be(1);
        result.Usage.TotalTokenCount.Should().Be(3);
        result.FinishReason.Should().NotBeNull();
        result.FinishReason!.Value.Should().Be(ChatFinishReason.Stop);
    }

    [Fact]
    public async Task MapToChatResponseUpdates_StreamsCorrectly()
    {
        // Arrange
        var responseDtos = new List<ChatCompletionResponseDto?>
        {
            new(
                [
                    new ChatChoiceDto(
                        0,
                        null,
                        new ChatDeltaDto("Hello", "assistant"))
                ],
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                "resp-2",
                "chat.completion.chunk",
                "gpt-4"
            ),
            null // Simulate a null in the stream
        };

        var asyncStream = GetAsyncEnumerable(responseDtos);

        // Act
        var updates = new List<ChatResponseUpdate>();
        await foreach (var update in _mapper.MapToChatResponseUpdates(asyncStream, CancellationToken.None))
        {
            updates.Add(update);
        }

        // Assert
        updates.Should().HaveCount(1);
        updates[0].Role.Should().Be(ChatRole.Assistant);
        updates[0].Contents.OfType<TextContent>().Single().Text.Should().Be("Hello");
    }

    [Fact]
    public void MapToChatCompletionRequestDto_NullOptions_HandlesGracefully()
    {
        // Arrange
        var messages = new[] { new ChatMessage(ChatRole.User, new List<AIContent> { new TextContent("Hello") }) };
        var domainModel = new ChatCompletionMapper.ChatCompletionRequest(messages, null);

        // Act
        var dto = _mapper.MapToChatCompletionRequestDto(domainModel);

        // Assert
        dto.Model.Should().BeNull();
        dto.Temperature.Should().BeNull();
        dto.MaxTokens.Should().BeNull();
        dto.TopP.Should().BeNull();
        dto.FrequencyPenalty.Should().BeNull();
        dto.PresencePenalty.Should().BeNull();
        dto.Stop.Should().BeNull();
        dto.Seed.Should().BeNull();
        dto.ParallelToolCalls.Should().BeNull();
    }

    private static async IAsyncEnumerable<ChatCompletionResponseDto?> GetAsyncEnumerable(
        IEnumerable<ChatCompletionResponseDto?> items)
    {
        foreach (var item in items)
        {
            yield return item;
            await Task.Delay(1); // Simulate async
        }
    }

    [Fact]
    public async Task MapToChatResponseUpdates_MessageDtoBranch_Covered()
    {
        // Arrange: chatChoiceDto.Message is not null, Delta is null
        var responseDtos = new List<ChatCompletionResponseDto?>
        {
            new(
                [
                    new ChatChoiceDto(
                        0,
                        new ChatMessageDto("assistant", new TextContentDto("Hello from messageDto!")))
                ],
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                "resp-messageDto",
                "chat.completion.chunk",
                "gpt-4"
            )
        };

        var asyncStream = GetAsyncEnumerable(responseDtos);

        // Act
        var updates = new List<ChatResponseUpdate>();
        await foreach (var update in _mapper.MapToChatResponseUpdates(asyncStream, CancellationToken.None))
        {
            updates.Add(update);
        }

        // Assert
        updates.Should().HaveCount(1);
        updates[0].Role.Should().Be(ChatRole.Assistant);
        updates[0].Contents.OfType<TextContent>().Single().Text.Should().Be("Hello from messageDto!");
    }

    [Fact]
    public async Task MapToChatResponseUpdates_AddContentIfNotNull_CoversIfBranch()
    {
        // Arrange: Usage is set, so MapToUsageContent returns non-null
        var usage = new UsageDto(3, 5, 8);
        var responseDtos = new List<ChatCompletionResponseDto?>
        {
            new(
                [
                    new ChatChoiceDto(
                        0,
                        null,
                        new ChatDeltaDto("delta", "assistant"))
                ],
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                "resp-usage",
                "chat.completion.chunk",
                "gpt-4",
                null,
                usage
            )
        };

        var asyncStream = GetAsyncEnumerable(responseDtos);

        // Act
        var updates = new List<ChatResponseUpdate>();
        await foreach (var update in _mapper.MapToChatResponseUpdates(asyncStream, CancellationToken.None))
        {
            updates.Add(update);
        }

        // Assert: The first content should be a UsageContent
        updates.Should().HaveCount(1);
        updates[0].Contents.Should().NotBeEmpty();
        updates[0].Contents[0].Should().BeOfType<UsageContent>();
        var usageContent = (UsageContent)updates[0].Contents[0];
        usageContent.Details.InputTokenCount.Should().Be(5);
        usageContent.Details.OutputTokenCount.Should().Be(3);
        usageContent.Details.TotalTokenCount.Should().Be(8);
    }

    [Fact]
    public void MapToMessageContentDto_HandlesTextReasoningContent()
    {
        var contents = new List<AIContent> { new TextReasoningContent("reasoning") };
        var result = typeof(ChatCompletionMapper)
            .GetMethod("MapToMessageContentDto", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(_mapper, [contents]);
        result.Should().BeOfType<TextContentDto>().Which.Text.Should().Contain("reasoning");
    }

    [Fact]
    public void MapToMessageContentDto_HandlesErrorContent_AndSkips()
    {
        var contents = new List<AIContent> { new ErrorContent("err") };
        var result = typeof(ChatCompletionMapper)
            .GetMethod("MapToMessageContentDto", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(_mapper, [contents]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToMessageContentDto_HandlesUriContent_Image()
    {
        var contents = new List<AIContent> { new UriContent(new Uri("http://img"), "image/png") };
        var result = typeof(ChatCompletionMapper)
            .GetMethod("MapToMessageContentDto", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(_mapper, [contents]);
        result.Should().BeOfType<MultipartContentDto>();
    }

    [Fact]
    public void MapToMessageContentDto_ThrowsOnUriContent_NonImage()
    {
        var contents = new List<AIContent> { new UriContent(new Uri("http://file"), "application/pdf") };
        Action act = () => typeof(ChatCompletionMapper)
            .GetMethod("MapToMessageContentDto", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(_mapper, [contents]);
        act.Should().Throw<TargetInvocationException>().WithInnerException<NotSupportedException>();
    }

    [Fact]
    public void MapToMessageContentDto_HandlesDataContent_Image()
    {
        var contents = new List<AIContent>
        {
            new DataContent(
                "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAANwAAADcCAYAAAAbWs+BAAAGwElEQVR4Ae3cwZFbNxBFUY5rkrDTmKAUk5QT03Aa44U22KC7NHptw+DRikVAXf8fzC3u8Hj4R4AAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAgZzAW26USQT+e4HPx+Mz+RRvj0e0kT+SD2cWAQK1gOBqH6sEogKCi3IaRqAWEFztY5VAVEBwUU7DCNQCgqt9rBKICgguymkYgVpAcLWPVQJRAcFFOQ0jUAsIrvaxSiAqILgop2EEagHB1T5WCUQFBBflNIxALSC42scqgaiA4KKchhGoBQRX+1glEBUQXJTTMAK1gOBqH6sEogKCi3IaRqAWeK+Xb1z9iN558fHxcSPS9p2ezx/ROz4e4TtIHt+3j/61hW9f+2+7/+UXbifjewIDAoIbQDWSwE5AcDsZ3xMYEBDcAKqRBHYCgtvJ+J7AgIDgBlCNJLATENxOxvcEBgQEN4BqJIGdgOB2Mr4nMCAguAFUIwnsBAS3k/E9gQEBwQ2gGklgJyC4nYzvCQwICG4A1UgCOwHB7WR8T2BAQHADqEYS2AkIbifjewIDAoIbQDWSwE5AcDsZ3xMYEEjfTzHwiK91B8npd6Q8n8/oGQ/ckRJ9vvQwv3BpUfMIFAKCK3AsEUgLCC4tah6BQkBwBY4lAmkBwaVFzSNQCAiuwLFEIC0guLSoeQQKAcEVOJYIpAUElxY1j0AhILgCxxKBtIDg0qLmESgEBFfgWCKQFhBcWtQ8AoWA4AocSwTSAoJLi5pHoBAQXIFjiUBaQHBpUfMIFAKCK3AsEUgLCC4tah6BQmDgTpPsHSTFs39p6fQ7Q770UsV/Ov19X+2OFL9wxR+rJQJpAcGlRc0jUAgIrsCxRCAtILi0qHkECgHBFTiWCKQFBJcWNY9AISC4AscSgbSA4NKi5hEoBARX4FgikBYQXFrUPAKFgOAKHEsE0gKCS4uaR6AQEFyBY4lAWkBwaVHzCBQCgitwLBFICwguLWoegUJAcAWOJQJpAcGlRc0jUAgIrsCxRCAt8J4eePq89B0ar3ZnyOnve/rfn1+400/I810lILirjtPLnC4guNNPyPNdJSC4q47Ty5wuILjTT8jzXSUguKuO08ucLiC400/I810lILirjtPLnC4guNNPyPNdJSC4q47Ty5wuILjTT8jzXSUguKuO08ucLiC400/I810lILirjtPLnC4guNNPyPNdJSC4q47Ty5wuILjTT8jzXSUguKuO08ucLiC400/I810l8JZ/m78+szP/zI47fJo7Q37vgJ7PHwN/07/3TOv/9gu3avhMYFhAcMPAxhNYBQS3avhMYFhAcMPAxhNYBQS3avhMYFhAcMPAxhNYBQS3avhMYFhAcMPAxhNYBQS3avhMYFhAcMPAxhNYBQS3avhMYFhAcMPAxhNYBQS3avhMYFhAcMPAxhNYBQS3avhMYFhAcMPAxhNYBQS3avhMYFhAcMPAxhNYBQS3avhMYFhg4P6H9J0maYHXuiMlrXf+vOfA33Turf3C5SxNItAKCK4lsoFATkBwOUuTCLQCgmuJbCCQExBcztIkAq2A4FoiGwjkBASXszSJQCsguJbIBgI5AcHlLE0i0AoIriWygUBOQHA5S5MItAKCa4lsIJATEFzO0iQCrYDgWiIbCOQEBJezNIlAKyC4lsgGAjkBweUsTSLQCgiuJbKBQE5AcDlLkwi0Akff//Dz6U+/I6U1/sUNr3bnytl3kPzi4bXb/cK1RDYQyAkILmdpEoFWQHAtkQ0EcgKCy1maRKAVEFxLZAOBnIDgcpYmEWgFBNcS2UAgJyC4nKVJBFoBwbVENhDICQguZ2kSgVZAcC2RDQRyAoLLWZpEoBUQXEtkA4GcgOByliYRaAUE1xLZQCAnILicpUkEWgHBtUQ2EMgJCC5naRKBVkBwLZENBHIC/4M7TXIv+3PS22d24qvdQfL3C/7N5P5i/MLlLE0i0AoIriWygUBOQHA5S5MItAKCa4lsIJATEFzO0iQCrYDgWiIbCOQEBJezNIlAKyC4lsgGAjkBweUsTSLQCgiuJbKBQE5AcDlLkwi0AoJriWwgkBMQXM7SJAKtgOBaIhsI5AQEl7M0iUArILiWyAYCOQHB5SxNItAKCK4lsoFATkBwOUuTCBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIDAvyrwDySEJ2VQgUSoAAAAAElFTkSuQmCC",
                "image/png")
        };

        var result = typeof(ChatCompletionMapper)
            .GetMethod("MapToMessageContentDto", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(_mapper, [contents]);
        result.Should().BeOfType<MultipartContentDto>();
    }

    [Fact]
    public void MapToMessageContentDto_ThrowsOnDataContent_NonImage()
    {
        var contents = new List<AIContent>
        {
            new DataContent(
                "data:image/png;base64,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",
                "toto/png")
        };
        Action act = () => typeof(ChatCompletionMapper)
            .GetMethod("MapToMessageContentDto", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(_mapper, [contents]);
        act.Should().Throw<TargetInvocationException>().WithInnerException<NotSupportedException>();
    }

    [Fact]
    public void MapToMessageContentDto_ThrowsOnUnsupportedType()
    {
        var contents = new List<AIContent> { new DummyContent() };
        Action act = () => typeof(ChatCompletionMapper)
            .GetMethod("MapToMessageContentDto", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(_mapper, [contents]);
        act.Should().Throw<TargetInvocationException>().WithInnerException<NotSupportedException>();
    }

    private sealed class DummyContent : AIContent
    {
    }

    [Fact]
    public void MapToToolCallDto_HandlesFunctionCallContent()
    {
        var funcCall = new FunctionCallContent("id", "func", new Dictionary<string, object?> { { "a", 1 } });
        var result = typeof(ChatCompletionMapper)
            .GetMethod("MapToToolCallDto", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, [funcCall]);
        result.Should().BeOfType<ToolCallDto>();
    }

    private sealed class DummyAiFunction : AIFunction
    {
        protected override ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments,
            CancellationToken cancellationToken)
        {
            return ValueTask.FromResult<object?>(null);
        }
    }

    [Fact]
    public void MapToToolDto_HandlesAIFunction()
    {
        var func = new DummyAiFunction();
        var result = typeof(ChatCompletionMapper)
            .GetMethod("MapToToolDto", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, [func]);
        result.Should().BeOfType<ToolDto>();
    }

    [Fact]
    public void MapToFunctionCallContent_HandlesToolCallDeltaDto()
    {
        var delta = new ToolCallDeltaDto(0, "id", "", new FunctionCallDeltaDto("func", "{\"x\":2}"));
        var method = typeof(ChatCompletionMapper)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .First(m => m.Name == "MapToFunctionCallContent" &&
                        m.GetParameters()[0].ParameterType == typeof(ToolCallDeltaDto));
        var result = method.Invoke(_mapper, [delta]);
        result.Should().BeOfType<FunctionCallContent>();
    }

    [Fact]
    public void MapToFunctionCallContent_HandlesToolCallDto()
    {
        var dto = new ToolCallDto("id", "function", new FunctionCallDto("func", "{\"y\":3}"));
        var method = typeof(ChatCompletionMapper)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .First(m => m.Name == "MapToFunctionCallContent" &&
                        m.GetParameters()[0].ParameterType == typeof(ToolCallDto));
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeOfType<FunctionCallContent>();
    }

    [Fact]
    public void MapToChatToolMode_ReturnsAutoChatToolMode_WhenTypeIsAuto()
    {
        var dto = new ToolChoiceDto { Type = "auto" };
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatToolMode", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeOfType<AutoChatToolMode>();
    }

    [Fact]
    public void MapToChatToolMode_ReturnsNoneChatToolMode_WhenTypeIsNone()
    {
        var dto = new ToolChoiceDto { Type = "none" };
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatToolMode", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeOfType<NoneChatToolMode>();
    }

    [Fact]
    public void MapToChatToolMode_ReturnsRequiredChatToolMode_WhenTypeIsFunction_WithFunction()
    {
        var dto = new ToolChoiceDto { Type = "function", Function = new ToolFunctionDto("myFunc") };
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatToolMode", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeOfType<RequiredChatToolMode>();
        ((RequiredChatToolMode)result).RequiredFunctionName.Should().Be("myFunc");
    }

    [Fact]
    public void MapToChatToolMode_ReturnsRequiredChatToolMode_WhenTypeIsFunction_WithoutFunction()
    {
        var dto = new ToolChoiceDto { Type = "function", Function = null };
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatToolMode", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeOfType<RequiredChatToolMode>();
        ((RequiredChatToolMode)result).RequiredFunctionName.Should().BeNull();
    }

    [Fact]
    public void MapToChatToolMode_ReturnsNull_WhenDtoIsNull()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatToolMode", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [null]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToChatToolMode_ReturnsNull_WhenTypeIsUnknown()
    {
        var dto = new ToolChoiceDto { Type = "unknown" };
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatToolMode", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToAiTool_ReturnsNull_WhenDtoIsNull()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToAiTool", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [null]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToIDictionaryStringNullableObject_ReturnsEmpty_WhenInputIsNull()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToIDictionaryStringNullableObject", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [null]);
        result.Should().BeOfType<Dictionary<string, object?>>();
        ((Dictionary<string, object?>)result).Should().BeEmpty();
    }

    [Fact]
    public void MapToIDictionaryStringNullableObject_ReturnsEmpty_WhenInputIsEmpty()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToIDictionaryStringNullableObject", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [""]);
        result.Should().BeOfType<Dictionary<string, object?>>();
        ((Dictionary<string, object?>)result).Should().BeEmpty();
    }

    [Fact]
    public void MapToIDictionaryStringNullableObject_ParsesValidJson()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToIDictionaryStringNullableObject", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, ["{\"foo\":123,\"bar\":null}"]);
        result.Should().BeOfType<Dictionary<string, object?>>();
        var dict = (Dictionary<string, object?>)result;
        dict.Should().ContainKey("foo");
        dict.Should().ContainKey("bar");
    }

    [Fact]
    public void MapToIDictionaryStringNullableObject_ReturnsEmpty_WhenJsonIsInvalid()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToIDictionaryStringNullableObject", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, ["{not valid json}"]);
        result.Should().BeOfType<Dictionary<string, object?>>();
        ((Dictionary<string, object?>)result).Should().BeEmpty();
    }

    [Fact]
    public void MapToChatFinishReason_ReturnsStop_WhenInputIsStop()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatFinishReason", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, ["stop"]);
        result.Should().Be(ChatFinishReason.Stop);
    }

    [Fact]
    public void MapToChatFinishReason_ReturnsLength_WhenInputIsLength()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatFinishReason", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, ["length"]);
        result.Should().Be(ChatFinishReason.Length);
    }

    [Fact]
    public void MapToChatFinishReason_ReturnsToolCalls_WhenInputIsToolCalls()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatFinishReason", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, ["tool_calls"]);
        result.Should().Be(ChatFinishReason.ToolCalls);
    }

    [Fact]
    public void MapToChatFinishReason_ReturnsContentFilter_WhenInputIsContentFilter()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatFinishReason", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, ["content_filter"]);
        result.Should().Be(ChatFinishReason.ContentFilter);
    }

    [Fact]
    public void MapToChatFinishReason_ReturnsNull_WhenInputIsNull()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatFinishReason", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [null]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToChatFinishReason_ReturnsCustom_WhenInputIsUnknown()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatFinishReason", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, ["custom_reason"]);
        result.Should().BeOfType<ChatFinishReason>();
        ((ChatFinishReason)result).Value.Should().Be("custom_reason");
    }

    [Fact]
    public void MapToToolChoiceDto_ReturnsAuto_WhenAutoChatToolMode()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToToolChoiceDto", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [new AutoChatToolMode()]);
        result.Should().Be(ToolChoiceDto.Auto);
    }

    [Fact]
    public void MapToToolChoiceDto_ReturnsRequired_WhenRequiredChatToolMode_WithoutFunction()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToToolChoiceDto", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [new RequiredChatToolMode(null)]);
        result.Should().Be(ToolChoiceDto.Required);
    }

    [Fact]
    public void MapToToolChoiceDto_ReturnsForFunction_WhenRequiredChatToolMode_WithFunction()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToToolChoiceDto", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [new RequiredChatToolMode("myFunc")]);
        result.Should().BeEquivalentTo(ToolChoiceDto.ForFunction("myFunc"));
    }

    [Fact]
    public void MapToToolChoiceDto_ReturnsNull_WhenNull()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToToolChoiceDto", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [null]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToChatRole_ReturnsSystem_WhenInputIsSystem()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatRole", BindingFlags.NonPublic | BindingFlags.Static)!;
        var result = method.Invoke(null, ["system", ChatRole.User]);
        result.Should().Be(ChatRole.System);
    }

    [Fact]
    public void MapToChatRole_ReturnsUser_WhenInputIsUser()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatRole", BindingFlags.NonPublic | BindingFlags.Static)!;
        var result = method.Invoke(null, ["user", ChatRole.Assistant]);
        result.Should().Be(ChatRole.User);
    }

    [Fact]
    public void MapToChatRole_ReturnsAssistant_WhenInputIsAssistant()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatRole", BindingFlags.NonPublic | BindingFlags.Static)!;
        var result = method.Invoke(null, ["assistant", ChatRole.User]);
        result.Should().Be(ChatRole.Assistant);
    }

    [Fact]
    public void MapToChatRole_ReturnsTool_WhenInputIsTool()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatRole", BindingFlags.NonPublic | BindingFlags.Static)!;
        var result = method.Invoke(null, ["tool", ChatRole.User]);
        result.Should().Be(ChatRole.Tool);
    }

    [Fact]
    public void MapToChatRole_ReturnsDefault_WhenInputIsNull()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatRole", BindingFlags.NonPublic | BindingFlags.Static)!;
        var result = method.Invoke(null, [null, ChatRole.Assistant]);
        result.Should().Be(ChatRole.Assistant);
    }

    [Fact]
    public void MapToChatRole_ReturnsDefault_WhenInputIsUnknown()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatRole", BindingFlags.NonPublic | BindingFlags.Static)!;
        var result = method.Invoke(null, ["unknown_role", ChatRole.User]);
        result.Should().Be(ChatRole.User);
    }

    [Fact]
    public void MapToTextContent_ReturnsTextContent_WhenTextContentDto()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToTextContent", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var dto = new TextContentDto("hello world");
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeOfType<TextContent>().Which.Text.Should().Be("hello world");
    }

    [Fact]
    public void MapToTextContent_ReturnsTextContent_WhenMultipartContentDto_WithTextParts()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToTextContent", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var dto = new MultipartContentDto([new TextPartDto("foo"), new TextPartDto("bar")]);
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeOfType<TextContent>().Which.Text.Should().Be("foo bar");
    }

    [Fact]
    public void MapToTextContent_ReturnsNull_WhenNull()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToTextContent", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [null]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToTextContent_ReturnsNull_WhenMultipartContentDto_HasNoTextParts()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToTextContent", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var dto = new MultipartContentDto([]);
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToTextContent_ReturnsEmpty_WhenMultipartContentDto_HasEmptyTextParts()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToTextContent", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var dto = new MultipartContentDto([new TextPartDto(""), new TextPartDto("")]);
        var result = method.Invoke(_mapper, [dto]);
        result.Should().NotBeNull();
    }

    [Fact]
    public void MapToChatResponseFormat_ReturnsText_WhenTypeIsText()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatResponseFormat", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var dto = new ResponseFormatDto("text");
        var result = method.Invoke(_mapper, [dto]);
        result.Should().Be(ChatResponseFormat.Text);
    }

    [Fact]
    public void MapToChatResponseFormat_ReturnsJsonObject_WhenTypeIsJsonObject_WithSchema()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatResponseFormat", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var schema = new JsonSchemaDto("object", "desc", new JsonElement());
        var dto = new ResponseFormatDto("json_object", schema);
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeOfType<ChatResponseFormatJson>();
    }

    [Fact]
    public void MapToChatResponseFormat_ReturnsJson_WhenTypeIsJsonObject_WithoutSchema()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatResponseFormat", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var dto = new ResponseFormatDto("json_object");
        var result = method.Invoke(_mapper, [dto]);
        result.Should().Be(ChatResponseFormat.Json);
    }

    [Fact]
    public void MapToChatResponseFormat_ReturnsNull_WhenInputIsNull()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatResponseFormat", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [null]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToChatResponseFormat_ReturnsNull_WhenTypeIsUnknown()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToChatResponseFormat", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var dto = new ResponseFormatDto("unknown_type");
        var result = method.Invoke(_mapper, [dto]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToResponseFormatDto_ReturnsText_WhenChatResponseFormatText()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToResponseFormatDto", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [ChatResponseFormat.Text]);
        result.Should().BeOfType<ResponseFormatDto>();
        ((ResponseFormatDto)result).Type.Should().Be("text");
    }

    [Fact]
    public void MapToResponseFormatDto_ReturnsJsonObject_WithoutSchema_WhenChatResponseFormatJsonWithoutSchema()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToResponseFormatDto", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var format = new ChatResponseFormatJson(null);
        var result = method.Invoke(_mapper, [format]);
        result.Should().BeOfType<ResponseFormatDto>();
        ((ResponseFormatDto)result).Type.Should().Be("json_object");
        ((ResponseFormatDto)result).JsonSchema.Should().BeNull();
    }

    [Fact]
    public void MapToResponseFormatDto_ReturnsNull_WhenInputIsNull()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToResponseFormatDto", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [null]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToTools_ReturnsNull_WhenInputIsNull()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToTools", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [null]);
        result.Should().BeNull();
    }

    [Fact]
    public void MapToTools_ReturnsEmpty_WhenInputIsEmpty()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToTools", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = method.Invoke(_mapper, [new List<ToolDto>()]);
        result.Should().BeAssignableTo<IEnumerable<AITool>>();
        ((IEnumerable<AITool>)result).Should().BeEmpty();
    }

    [Fact]
    public void MapToTools_MapsValidToolDto()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToTools", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var toolDto = new ToolDto("function", new FunctionDefinitionDto(string.Empty));
        var result = method.Invoke(_mapper, [new List<ToolDto> { toolDto }]);
        result.Should().BeAssignableTo<IEnumerable<AITool>>();
        ((IEnumerable<AITool>)result).Should().BeEmpty();
    }

    [Fact]
    public void MapToMessageContentDto_AddsFunctionResultContentResultVariants()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethod("MapToMessageContentDto", BindingFlags.NonPublic | BindingFlags.Instance)!;

        // Result is a string
        var contentsString = new List<AIContent> { new FunctionResultContent("id", "result string") };
        var resultString = method.Invoke(_mapper, [contentsString]);
        resultString.Should().BeOfType<TextContentDto>();
        ((TextContentDto)resultString).Text.Should().Contain("result string");

        // Result is null
        var contentsNull = new List<AIContent> { new FunctionResultContent("id", null) };
        var resultNull = method.Invoke(_mapper, [contentsNull]);
        resultNull.Should().BeNull();

        // Result is an object
        var obj = new { foo = 42, bar = "baz" };
        var contentsObj = new List<AIContent> { new FunctionResultContent("id", obj) };
        var resultObj = method.Invoke(_mapper, [contentsObj]);
        resultObj.Should().BeOfType<TextContentDto>();
        ((TextContentDto)resultObj).Text.Should().Contain("foo");
        ((TextContentDto)resultObj).Text.Should().Contain("bar");
    }

    [Fact]
    public void MapToChatMessageDtos_YieldsSystemMessage_WhenInstructionsProvided()
    {
        var method = typeof(ChatCompletionMapper)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .First(m =>
                m.Name == "MapToChatMessageDtos" &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType == typeof(IEnumerable<ChatMessage>) &&
                m.GetParameters()[1].ParameterType == typeof(ChatOptions)
            );
        var messages = new List<ChatMessage>();
        var options = new ChatOptions { Instructions = "Follow these instructions." };

        var result =
            ((IEnumerable<ChatMessageDto>)method.Invoke(_mapper, [messages, options])!).ToList();

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result[0].Role.Should().Be("system");
        result[0].Content.Should().BeOfType<TextContentDto>();
        ((TextContentDto)result[0].Content!).Text.Should().Be("Follow these instructions.");
    }
}
