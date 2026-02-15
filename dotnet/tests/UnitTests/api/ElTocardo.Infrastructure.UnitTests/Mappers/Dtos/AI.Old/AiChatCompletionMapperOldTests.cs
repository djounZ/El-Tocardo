using ElTocardo.Application.Dtos.ChatCompletion;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.Old;

public class AiChatCompletionMapperOldTests
{
	private readonly Mock<ILogger<AiChatCompletionMapperOld>> _loggerMock = new();
	private readonly AiContentMapperOld _contentMapper = new(new Mock<ILogger<AiContentMapperOld>>().Object);
	private readonly AiChatCompletionMapperOld _mapper;

	public AiChatCompletionMapperOldTests()
	{
		_mapper = new AiChatCompletionMapperOld(_loggerMock.Object, _contentMapper);
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
		var method = typeof(AiChatCompletionMapperOld).GetMethod("MapToFinishReasonDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [null]);
		result.Should().BeNull();
	}

	[Fact]
	public void MapToFinishReasonDto_ReturnsStop_WhenInputIsStop()
	{
		var method = typeof(AiChatCompletionMapperOld).GetMethod("MapToFinishReasonDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [Microsoft.Extensions.AI.ChatFinishReason.Stop]);
		result.Should().Be(ChatFinishReasonDto.Stop);
	}

	[Fact]
	public void MapToFinishReasonDto_ReturnsLength_WhenInputIsLength()
	{
		var method = typeof(AiChatCompletionMapperOld).GetMethod("MapToFinishReasonDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [Microsoft.Extensions.AI.ChatFinishReason.Length]);
		result.Should().Be(ChatFinishReasonDto.Length);
	}

	[Fact]
	public void MapToFinishReasonDto_ReturnsToolCalls_WhenInputIsToolCalls()
	{
		var method = typeof(AiChatCompletionMapperOld).GetMethod("MapToFinishReasonDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [Microsoft.Extensions.AI.ChatFinishReason.ToolCalls]);
		result.Should().Be(ChatFinishReasonDto.ToolCalls);
	}

	[Fact]
	public void MapToFinishReasonDto_ReturnsContentFilter_WhenInputIsContentFilter()
	{
		var method = typeof(AiChatCompletionMapperOld).GetMethod("MapToFinishReasonDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [Microsoft.Extensions.AI.ChatFinishReason.ContentFilter]);
		result.Should().Be(ChatFinishReasonDto.ContentFilter);
	}	[Fact]
	public void MapToChatResponseFormatDto_ReturnsNull_WhenInputIsNull()
	{
		var method = typeof(AiChatCompletionMapperOld).GetMethod("MapToChatResponseFormatDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var result = method!.Invoke(_mapper, [null]);
		result.Should().BeNull();
	}

	[Fact]
	public void MapToChatResponseFormatDto_ReturnsTextDto_WhenInputIsText()
	{
		var method = typeof(AiChatCompletionMapperOld).GetMethod("MapToChatResponseFormatDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var textFormat = new Microsoft.Extensions.AI.ChatResponseFormatText();
		var result = method!.Invoke(_mapper, [textFormat]);
		result.Should().BeOfType<ChatResponseFormatTextDto>();
	}

	[Fact]
	public void MapToChatResponseFormatDto_ReturnsJsonDto_WhenInputIsJson()
	{
		var method = typeof(AiChatCompletionMapperOld).GetMethod("MapToChatResponseFormatDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var jsonFormat = new Microsoft.Extensions.AI.ChatResponseFormatJson(System.Text.Json.JsonDocument.Parse("{}").RootElement, "schemaName", "desc");
		var result = method!.Invoke(_mapper, [jsonFormat]);
		result.Should().BeOfType<ChatResponseFormatJsonDto>();
	}




}
