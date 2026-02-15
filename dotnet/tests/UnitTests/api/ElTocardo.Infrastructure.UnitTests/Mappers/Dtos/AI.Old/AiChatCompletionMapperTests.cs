using ElTocardo.Application.Dtos.ChatCompletion;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.Old;

public class AiChatCompletionMapperTests
{
	private readonly Mock<ILogger<AiChatCompletionMapper>> _loggerMock = new();
	private readonly AiContentMapperOld _contentMapper = new(new Mock<ILogger<AiContentMapperOld>>().Object);
	private readonly AiChatCompletionMapper _mapper;

	public AiChatCompletionMapperTests()
	{
		_mapper = new AiChatCompletionMapper(_loggerMock.Object, _contentMapper);
	}

	[Fact]
	public void MapToAiChatClientRequest_MapsCorrectly()
	{
		// Arrange
		var messageDto = new ChatMessageDto(ChatRoleEnumDto.User, new List<AiContentDto>());
		var optionsDto = new ChatOptionsDto(
			ConversationId: "conv1",
			Instructions: "instr",
			Temperature: 0.5f,
			MaxOutputTokens: 100,
			TopP: 0.9f,
			TopK: 10,
			FrequencyPenalty: 0.1f,
			PresencePenalty: 0.2f,
			Seed: 42,
            Reasoning: null, //todo
			ResponseFormat: null,
			ModelId: "model",
			StopSequences: new List<string> { "stop" },
			AllowMultipleToolCalls: true,
			ToolMode: null,
			Tools: null
		);
		var dto = new ChatRequestDto([messageDto], null, optionsDto);

		// Act
		var result = _mapper.MapToAiChatClientRequest(dto);

		// Assert
		result.Messages.Should().HaveCount(1);
		result.Options.Should().NotBeNull();
		result.Options!.ConversationId.Should().Be("conv1");
		result.Options.Instructions.Should().Be("instr");
		result.Options.Temperature.Should().Be(0.5f);
		result.Options.MaxOutputTokens.Should().Be(100);
		result.Options.TopP.Should().Be(0.9f);
		result.Options.TopK.Should().Be(10);
		result.Options.FrequencyPenalty.Should().Be(0.1f);
		result.Options.PresencePenalty.Should().Be(0.2f);
		result.Options.Seed.Should().Be(42);
		result.Options.ModelId.Should().Be("model");
		result.Options.StopSequences.Should().ContainSingle(s => s == "stop");
		result.Options.AllowMultipleToolCalls.Should().BeTrue();
	}	[Fact]
	public void MapToChatClientRequestDto_MapsCorrectly()
	{
		// Arrange
		var aiMessages = new[]
		{
			new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, [new Microsoft.Extensions.AI.TextContent("hi")
            ])
		};
		var options = new Microsoft.Extensions.AI.ChatOptions
		{
			ConversationId = "conv1",
			Instructions = "instr",
			Temperature = 0.5f,
			MaxOutputTokens = 100,
			TopP = 0.9f,
			TopK = 10,
			FrequencyPenalty = 0.1f,
			PresencePenalty = 0.2f,
			Seed = 42,
			ModelId = "model",
			StopSequences = new List<string> { "stop" },
			AllowMultipleToolCalls = true
		};
		var aiRequest = new AiChatClientRequest(aiMessages, options);
		var provider = AiProviderEnumDto.GithubCopilot;

		// Act
		var dto = _mapper.MapToChatClientRequestDto(aiRequest, provider);

		// Assert
		dto.Messages.Should().HaveCount(1);
		dto.Provider.Should().Be(provider);
		dto.Options.Should().NotBeNull();
		dto.Options!.ConversationId.Should().Be("conv1");
		dto.Options.Instructions.Should().Be("instr");
		dto.Options.Temperature.Should().Be(0.5f);
		dto.Options.MaxOutputTokens.Should().Be(100);
		dto.Options.TopP.Should().Be(0.9f);
		dto.Options.TopK.Should().Be(10);
		dto.Options.FrequencyPenalty.Should().Be(0.1f);
		dto.Options.PresencePenalty.Should().Be(0.2f);
		dto.Options.Seed.Should().Be(42);
		dto.Options.ModelId.Should().Be("model");
		dto.Options.StopSequences.Should().ContainSingle(s => s == "stop");
		dto.Options.AllowMultipleToolCalls.Should().BeTrue();
	}

	[Fact]
	public void MapToAiChatClientRequest_NullOptions_MapsCorrectly()
	{
		var messageDto = new ChatMessageDto(ChatRoleEnumDto.User, new List<AiContentDto>());
		var dto = new ChatRequestDto([messageDto]);
		var result = _mapper.MapToAiChatClientRequest(dto);
		result.Options.Should().BeNull();
	}

	[Fact]
	public void MapToChatClientRequestDto_NullOptions_MapsCorrectly()
	{
		var aiMessages = new[]
		{
			new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, [new Microsoft.Extensions.AI.TextContent("hi")
            ])
		};
		var aiRequest = new AiChatClientRequest(aiMessages);
		var provider = AiProviderEnumDto.GithubCopilot;
		var dto = _mapper.MapToChatClientRequestDto(aiRequest, provider);
		dto.Options.Should().BeNull();
	}

	[Fact]
	public void MapToChatResponseDto_AndBack_MapsCorrectly()
	{
		// Arrange
		var chatResponse = new Microsoft.Extensions.AI.ChatResponse([
            new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, [new Microsoft.Extensions.AI.TextContent("hi")
            ])
        ])
		{
			ResponseId = "resp1",
			ConversationId = "conv1",
			ModelId = "model",
			CreatedAt = DateTimeOffset.UtcNow,
			FinishReason = Microsoft.Extensions.AI.ChatFinishReason.Stop
		};

		// Act
		var dto = _mapper.MapToChatResponseDto(chatResponse);
		var roundTrip = _mapper.MapToAiChatClientRequest(dto);

		// Assert
		dto.ResponseId.Should().Be("resp1");
		dto.ConversationId.Should().Be("conv1");
		dto.ModelId.Should().Be("model");
		dto.FinishReason.Should().Be(ChatFinishReasonDto.Stop);
		roundTrip.ResponseId.Should().Be("resp1");
		roundTrip.ConversationId.Should().Be("conv1");
		roundTrip.ModelId.Should().Be("model");
		roundTrip.FinishReason.Should().Be(Microsoft.Extensions.AI.ChatFinishReason.Stop);
	}

	[Fact]
	public void MapToChatResponseUpdateDto_AndBack_MapsCorrectly()
	{
		// Arrange
		var update = new Microsoft.Extensions.AI.ChatResponseUpdate
		{
			AuthorName = "author",
			Role = Microsoft.Extensions.AI.ChatRole.Assistant,
			Contents = [new Microsoft.Extensions.AI.TextContent("hi")],
			ResponseId = "resp1",
			MessageId = "msg1",
			ConversationId = "conv1",
			CreatedAt = DateTimeOffset.UtcNow,
			FinishReason = Microsoft.Extensions.AI.ChatFinishReason.Stop,
			ModelId = "model"
		};

		// Act
		var dto = _mapper.MapToChatResponseUpdateDto(update);
		var roundTrip = _mapper.MapToChatResponseUpdate(dto);

		// Assert
		dto.AuthorName.Should().Be("author");
		dto.Role.Should().Be(ChatRoleEnumDto.Assistant);
		dto.ResponseId.Should().Be("resp1");
		dto.MessageId.Should().Be("msg1");
		dto.ConversationId.Should().Be("conv1");
		dto.ModelId.Should().Be("model");
		roundTrip.AuthorName.Should().Be("author");
		roundTrip.Role.Should().Be(Microsoft.Extensions.AI.ChatRole.Assistant);
		roundTrip.ResponseId.Should().Be("resp1");
		roundTrip.MessageId.Should().Be("msg1");
		roundTrip.ConversationId.Should().Be("conv1");
		roundTrip.ModelId.Should().Be("model");
	}	[Fact]
	public void MapToFinishReasonDto_ReturnsNull_WhenInputIsNull()
	{
		var method = typeof(AiChatCompletionMapper).GetMethod("MapToFinishReasonDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [null]);
		result.Should().BeNull();
	}

	[Fact]
	public void MapToFinishReasonDto_ReturnsStop_WhenInputIsStop()
	{
		var method = typeof(AiChatCompletionMapper).GetMethod("MapToFinishReasonDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [Microsoft.Extensions.AI.ChatFinishReason.Stop]);
		result.Should().Be(ChatFinishReasonDto.Stop);
	}

	[Fact]
	public void MapToFinishReasonDto_ReturnsLength_WhenInputIsLength()
	{
		var method = typeof(AiChatCompletionMapper).GetMethod("MapToFinishReasonDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [Microsoft.Extensions.AI.ChatFinishReason.Length]);
		result.Should().Be(ChatFinishReasonDto.Length);
	}

	[Fact]
	public void MapToFinishReasonDto_ReturnsToolCalls_WhenInputIsToolCalls()
	{
		var method = typeof(AiChatCompletionMapper).GetMethod("MapToFinishReasonDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [Microsoft.Extensions.AI.ChatFinishReason.ToolCalls]);
		result.Should().Be(ChatFinishReasonDto.ToolCalls);
	}

	[Fact]
	public void MapToFinishReasonDto_ReturnsContentFilter_WhenInputIsContentFilter()
	{
		var method = typeof(AiChatCompletionMapper).GetMethod("MapToFinishReasonDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [Microsoft.Extensions.AI.ChatFinishReason.ContentFilter]);
		result.Should().Be(ChatFinishReasonDto.ContentFilter);
	}	[Fact]
	public void MapToChatResponseFormatDto_ReturnsNull_WhenInputIsNull()
	{
		var method = typeof(AiChatCompletionMapper).GetMethod("MapToChatResponseFormatDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [null]);
		result.Should().BeNull();
	}

	[Fact]
	public void MapToChatResponseFormatDto_ReturnsTextDto_WhenInputIsText()
	{
		var method = typeof(AiChatCompletionMapper).GetMethod("MapToChatResponseFormatDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var textFormat = new Microsoft.Extensions.AI.ChatResponseFormatText();
		var result = method!.Invoke(_mapper, [textFormat]);
		result.Should().BeOfType<ChatResponseFormatTextDto>();
	}

	[Fact]
	public void MapToChatResponseFormatDto_ReturnsJsonDto_WhenInputIsJson()
	{
		var method = typeof(AiChatCompletionMapper).GetMethod("MapToChatResponseFormatDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var jsonFormat = new Microsoft.Extensions.AI.ChatResponseFormatJson(System.Text.Json.JsonDocument.Parse("{}").RootElement, "schemaName", "desc");
		var result = method!.Invoke(_mapper, [jsonFormat]);
		result.Should().BeOfType<ChatResponseFormatJsonDto>();
	}




}
