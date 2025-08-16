using AI.GithubCopilot.Infrastructure.Dtos;
using AI.GithubCopilot.Infrastructure.Dtos.ChatCompletion;
using AI.GithubCopilot.Infrastructure.Services;
using AI.GithubCopilot.Options;
using AI.GithubCopilot.UnitTests.Infrastructure.Utils;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AI.GithubCopilot.UnitTests.Infrastructure.Services;

public class GithubCopilotChatCompletionTests
{
    private readonly Mock<ILogger<GithubCopilotChatCompletion>> _loggerMock = new();
    private readonly HttpClientRunner _httpClientRunner = new();
    private readonly Mock<IOptions<AiGithubOptions>> _optionsMock = new();

    private record GithubCopilotChatCompletionTextContext(
        GithubCopilotChatCompletion GithubCopilotChatCompletion,
        HttpClient HttpClient);

    private GithubCopilotChatCompletionTextContext CreateContext(MockHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler);
        GithubCopilotChatCompletion githubCopilotChatCompletion =
            new(_loggerMock.Object, httpClient, _optionsMock.Object, _httpClientRunner);

        return new GithubCopilotChatCompletionTextContext(githubCopilotChatCompletion, httpClient);
    }

    [Fact]
    public async Task GetModelsAsync_ReturnsModelsResponseDto()
    {
        var mockModelInfo = new ModelInfoDto(
            "model-123",
            "Test Model",
            "model",
            "1.0.0",
            "TestVendor",
            true,
            false,
            false,
            true,
            new BillingInfoDto(
                true,
                1.5m,
                new List<string> { "org1", "org2" }
            ),
            new ModelCapabilitiesDto(
                "test-family",
                "test-type",
                "test-tokenizer",
                "capabilities",
                new ModelLimitsDto(
                    4096,
                    1024,
                    2048,
                    10,
                    new ModelVisionLimitsDto(
                        512,
                        4,
                        new List<string> { "image/png", "image/jpeg" }
                    )
                ),
                new ModelSupportsDto(
                    true,
                    true,
                    false,
                    true,
                    true,
                    false
                )
            ),
            new ModelPolicyDto(
                "active",
                "Test terms"
            )
        );

        // Arrange
        var expected = new ModelsResponseDto([mockModelInfo], string.Empty);

        var handler = MockHttpMessageHandler.CreateWithJsonContent(expected);
        _optionsMock.Setup(o => o.Value).Returns(new AiGithubOptions
        {
            CopilotModelsUrl = "https://api.example.com/models",
            CopilotChatCompletionsHeaders = new Dictionary<string, string>()
        });

        var context = CreateContext(handler);
        try
        {
            // Act
            var result = await context.GithubCopilotChatCompletion.GetModelsAsync(CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
        finally
        {
            context.HttpClient.Dispose();
        }
    }

    [Fact]
    public async Task GetChatCompletionAsync_ReturnsChatCompletionResponseDto()
    {
        // Arrange
        var expected = new ChatCompletionResponseDto(
            Id: "chatcmpl-123",
            Object: "chat.completion",
            Created: 1234567890,
            Model: "test-model",
            Choices: new List<ChatChoiceDto>
            {
                new(
                    0,
                    new ChatMessageDto("assistant", new TextContentDto("Hello!")),
                    null,
                    "stop"
                )
            },
            Usage: new UsageDto(10, 20, 30)
        );

        var handler = MockHttpMessageHandler.CreateWithJsonContent(expected);

        _optionsMock.Setup(o => o.Value).Returns(new AiGithubOptions
        {
            CopilotChatCompletionsUrl = "https://api.example.com/chat/completions",
            CopilotChatCompletionsHeaders = new Dictionary<string, string>()
        });

        var context = CreateContext(handler);

        var request = new ChatCompletionRequestDto(
            Model: "test-model",
            Messages: new List<ChatMessageDto> { new("user", new TextContentDto("Hi!")) },
            Stream: false
        );

        try
        {
            // Act
            var result =
                await context.GithubCopilotChatCompletion.GetChatCompletionAsync(request, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
        finally
        {
            context.HttpClient.Dispose();
        }
    }

    [Fact]
    public async Task GetChatCompletionStreamAsync_ReturnsStreamedChatCompletionResponseDtos()
    {
        // Arrange
        var streamedResponses = new[]
        {
            new ChatCompletionResponseDto(
                Id: "chatcmpl-1",
                Object: "chat.completion.chunk",
                Created: 1234567890,
                Model: "test-model",
                Choices: new List<ChatChoiceDto>
                {
                    new(
                        0,
                        new ChatMessageDto("assistant", new TextContentDto("Hello!"))
                    )
                },
                Usage: null
            ),
            new ChatCompletionResponseDto(
                Id: "chatcmpl-2",
                Object: "chat.completion.chunk",
                Created: 1234567891,
                Model: "test-model",
                Choices: new List<ChatChoiceDto>
                {
                    new(
                        0,
                        new ChatMessageDto("assistant", new TextContentDto("How can I help?")),
                        null,
                        "stop"
                    )
                },
                Usage: null
            )
        };

        var handler = MockHttpMessageHandler.CreateWithStreamedJsonContent(streamedResponses);

        _optionsMock.Setup(o => o.Value).Returns(new AiGithubOptions
        {
            CopilotChatCompletionsUrl = "https://api.example.com/chat/completions",
            CopilotChatCompletionsHeaders = new Dictionary<string, string>()
        });

        var context = CreateContext(handler);

        var request = new ChatCompletionRequestDto(
            Model: "test-model",
            Messages: new List<ChatMessageDto> { new("user", new TextContentDto("Hi!")) },
            Stream: true
        );

        try
        {
            // Act
            var resultStream =
                context.GithubCopilotChatCompletion.GetChatCompletionStreamAsync(request, CancellationToken.None);

            // Assert
            var results = new List<ChatCompletionResponseDto>();
            await foreach (var item in resultStream)
            {
                results.Add(item!);
            }

            results.Should().BeEquivalentTo(streamedResponses);
        }
        finally
        {
            context.HttpClient.Dispose();
        }
    }

    [Fact]
    public async Task GetChatCompletionStreamAsync_AccumulatesToolCallArguments()
    {
        // Arrange: two streamed responses with the same ID and tool call, arguments split
        var toolCallId = "tool-1";
        var firstChunk = new ChatCompletionResponseDto(
            Id: "chatcmpl-accum",
            Object: "chat.completion.chunk",
            Created: 1234567890,
            Model: "test-model",
            Choices: new List<ChatChoiceDto>
            {
                new(
                    0,
                    null,
                    new ChatDeltaDto(
                        ToolCalls: new List<ToolCallDeltaDto>
                        {
                            new(
                                0,
                                toolCallId,
                                "function",
                                new FunctionCallDeltaDto(
                                    "myFunc",
                                    "part1"
                                )
                            )
                        }
                    )
                )
            },
            Usage: null
        );

        var secondChunk = new ChatCompletionResponseDto(
            Id: "chatcmpl-accum",
            Object: "chat.completion.chunk",
            Created: 1234567891,
            Model: "test-model",
            Choices: new List<ChatChoiceDto>
            {
                new(
                    0,
                    null,
                    new ChatDeltaDto(
                        ToolCalls: new List<ToolCallDeltaDto>
                        {
                            new(
                                1,
                                toolCallId,
                                "function",
                                new FunctionCallDeltaDto(
                                    "myFunc",
                                    "part2"
                                )
                            )
                        }
                    )
                )
            },
            Usage: null
        );

        // End chunk to flush the accumulated response
        var endChunk = new ChatCompletionResponseDto(
            Id: "chatcmpl-accum",
            Object: "chat.completion.chunk",
            Created: 1234567892,
            Model: "test-model",
            Choices: new List<ChatChoiceDto>
            {
                new(
                    0,
                    null,
                    new ChatDeltaDto(
                        ToolCalls: new List<ToolCallDeltaDto>
                        {
                            new(
                                2,
                                toolCallId,
                                "function",
                                new FunctionCallDeltaDto(
                                    "myFunc",
                                    ""
                                )
                            )
                        }
                    ),
                    "stop"
                )
            },
            Usage: null
        );

        var streamedResponses = new[] { firstChunk, secondChunk, endChunk };

        var handler = MockHttpMessageHandler.CreateWithStreamedJsonContent(streamedResponses, endOfStream: "[DONE]");

        _optionsMock.Setup(o => o.Value).Returns(new AiGithubOptions
        {
            CopilotChatCompletionsUrl = "https://api.example.com/chat/completions",
            CopilotChatCompletionsHeaders = new Dictionary<string, string>()
        });

        var context = CreateContext(handler);

        var request = new ChatCompletionRequestDto(
            Model: "test-model",
            Messages: new List<ChatMessageDto> { new("user", new TextContentDto("Hi!")) },
            Stream: true
        );

        try
        {
            // Act
            var resultStream =
                context.GithubCopilotChatCompletion.GetChatCompletionStreamAsync(request, CancellationToken.None);

            // Assert
            var results = new List<ChatCompletionResponseDto>();
            await foreach (var item in resultStream)
            {
                if (item != null)
                {
                    results.Add(item);
                }
            }

            // Only the last accumulated chunk should be yielded
            results.Should().HaveCount(1);
            var accumulated = results[0];
            var accumulatedArgs = accumulated.Choices[0].Delta!.ToolCalls![0].Function!.Arguments;
            accumulatedArgs.Should().Be("part1part2");
        }
        finally
        {
            context.HttpClient.Dispose();
        }
    }

    [Fact]
    public async Task GetChatCompletionStreamAsync_YieldsOnToolCallIdChange()
    {
        // Arrange: two tool call chunks with different IDs
        var firstToolCallId = "tool-1";
        var secondToolCallId = "tool-2";
        var firstChunk = new ChatCompletionResponseDto(
            Id: firstToolCallId,
            Object: "chat.completion.chunk",
            Created: 1234567890,
            Model: "test-model",
            Choices: new List<ChatChoiceDto>
            {
                new(
                    0,
                    null,
                    new ChatDeltaDto(
                        ToolCalls: new List<ToolCallDeltaDto>
                        {
                            new(
                                0,
                                firstToolCallId,
                                "function",
                                new FunctionCallDeltaDto(
                                    "funcA",
                                    "foo"
                                )
                            )
                        }
                    )
                )
            },
            Usage: null
        );

        var secondChunk = new ChatCompletionResponseDto(
            Id: secondToolCallId,
            Object: "chat.completion.chunk",
            Created: 1234567891,
            Model: "test-model",
            Choices: new List<ChatChoiceDto>
            {
                new(
                    0,
                    null,
                    new ChatDeltaDto(
                        ToolCalls: new List<ToolCallDeltaDto>
                        {
                            new(
                                0,
                                secondToolCallId,
                                "function",
                                new FunctionCallDeltaDto(
                                    "funcB",
                                    "bar"
                                )
                            )
                        }
                    )
                )
            },
            Usage: null
        );

        // End chunk to flush the last response
        var endChunk = new ChatCompletionResponseDto(
            Id: secondToolCallId,
            Object: "chat.completion.chunk",
            Created: 1234567892,
            Model: "test-model",
            Choices: new List<ChatChoiceDto>
            {
                new(
                    0,
                    null,
                    new ChatDeltaDto(
                        ToolCalls: new List<ToolCallDeltaDto>
                        {
                            new(
                                0,
                                secondToolCallId,
                                "function",
                                new FunctionCallDeltaDto(
                                    "funcB",
                                    ""
                                )
                            )
                        }
                    ),
                    "stop"
                )
            },
            Usage: null
        );

        var streamedResponses = new[] { firstChunk, secondChunk, endChunk };

        var handler = MockHttpMessageHandler.CreateWithStreamedJsonContent(streamedResponses, endOfStream: "[DONE]");

        _optionsMock.Setup(o => o.Value).Returns(new AiGithubOptions
        {
            CopilotChatCompletionsUrl = "https://api.example.com/chat/completions",
            CopilotChatCompletionsHeaders = new Dictionary<string, string>()
        });

        var context = CreateContext(handler);

        var request = new ChatCompletionRequestDto(
            Model: "test-model",
            Messages: new List<ChatMessageDto> { new("user", new TextContentDto("Hi!")) },
            Stream: true
        );

        try
        {
            // Act
            var resultStream =
                context.GithubCopilotChatCompletion.GetChatCompletionStreamAsync(request, CancellationToken.None);

            // Assert
            var results = new List<ChatCompletionResponseDto>();
            await foreach (var item in resultStream)
            {
                if (item != null)
                {
                    results.Add(item);
                }
            }

            // Should yield twice: once for each tool call ID
            results.Should().HaveCount(2);

            results[0].Id.Should().Be(firstToolCallId);
            results[0].Choices[0].Delta!.ToolCalls![0].Function!.Arguments.Should().Be("foo");

            results[1].Id.Should().Be(secondToolCallId);
            results[1].Choices[0].Delta!.ToolCalls![0].Function!.Arguments.Should().Be("bar");
        }
        finally
        {
            context.HttpClient.Dispose();
        }
    }[Fact]
public async Task GetChatCompletionStreamAsync_WithVisionMessage_SetsVisionHeaderAndStreams()
{
    // Arrange: create a message with an image part
    var imagePart = new ImagePartDto(
        new ImageUrlDto("https://example.com/image.png", "high")
    );
    var multipartContent = new MultipartContentDto([imagePart] );
    var visionMessage = new ChatMessageDto("user", multipartContent);

    var streamedResponses = new[]
    {
        new ChatCompletionResponseDto(
            Id: "vision-1",
            Object: "chat.completion.chunk",
            Created: 1234567890,
            Model: "test-model",
            Choices: new List<ChatChoiceDto>
            {
                new(
                    0,
                    new ChatMessageDto("assistant", new TextContentDto("I see an image.")),
                    null,
                    "stop"
                )
            },
            Usage: null
        )
    };

    var handler = MockHttpMessageHandler.CreateWithStreamedJsonContent(streamedResponses, endOfStream: "[DONE]");

    _optionsMock.Setup(o => o.Value).Returns(new AiGithubOptions
    {
        CopilotChatCompletionsUrl = "https://api.example.com/chat/completions",
        CopilotChatCompletionsHeaders = new Dictionary<string, string>()
    });

    var context = CreateContext(handler);

    var request = new ChatCompletionRequestDto(
        Model: "test-model",
        Messages: new List<ChatMessageDto> { visionMessage },
        Stream: true
    );

    try
    {
        // Act
        var resultStream = context.GithubCopilotChatCompletion.GetChatCompletionStreamAsync(request, CancellationToken.None);

        // Assert
        var results = new List<ChatCompletionResponseDto>();
        await foreach (var item in resultStream)
        {
            if (item != null)
                results.Add(item);
        }

        results.Should().HaveCount(1);
        results[0].Choices[0].Message!.Content.Should().BeOfType<TextContentDto>();
        results[0].Choices[0].Message!.Content.As<TextContentDto>().Text.Should().Be("I see an image.");

    }
    finally
    {
        context.HttpClient.Dispose();
    }
}
}
