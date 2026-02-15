using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ChatOptionsMapperTests
{
    private readonly Mock<IDomainEntityMapper<ReasoningOptions, ReasoningOptionsDto>> _reasoningOptionsMapperMock;
    private readonly Mock<IDomainEntityMapper<ChatResponseFormat, ChatResponseFormatDto>> _chatResponseFormatMapperMock;
    private readonly Mock<IDomainEntityMapper<ChatToolMode, ChatToolModeDto>> _chatToolModeMapperMock;
    private readonly Mock<IDomainEntityMapper<AITool, AbstractAiToolDto>> _aiToolMapperMock;
    private readonly IDomainEntityMapper<ChatOptions, ChatOptionsDto> _mapper;

    public ChatOptionsMapperTests()
    {
        _reasoningOptionsMapperMock = new Mock<IDomainEntityMapper<ReasoningOptions, ReasoningOptionsDto>>();
        _chatResponseFormatMapperMock = new Mock<IDomainEntityMapper<ChatResponseFormat, ChatResponseFormatDto>>();
        _chatToolModeMapperMock = new Mock<IDomainEntityMapper<ChatToolMode, ChatToolModeDto>>();
        _aiToolMapperMock = new Mock<IDomainEntityMapper<AITool, AbstractAiToolDto>>();

        _mapper = new ChatOptionsMapper(
            _reasoningOptionsMapperMock.Object,
            _chatResponseFormatMapperMock.Object,
            _chatToolModeMapperMock.Object,
            _aiToolMapperMock.Object
        );
    }

    [Fact]
    public void ToApplication_AllPropertiesSet_MapsCorrectly()
    {
        var reasoningOptions = new ReasoningOptions { Effort = ReasoningEffort.High };
        var responseFormat = new ChatResponseFormatText();
        var toolMode = new AutoChatToolMode();
        var tool = new HostedWebSearchTool();

        var domain = new ChatOptions
        {
            ConversationId = "conv-123",
            Instructions = "Test instructions",
            Temperature = 0.7f,
            MaxOutputTokens = 1000,
            TopP = 0.9f,
            TopK = 50,
            FrequencyPenalty = 0.5f,
            PresencePenalty = 0.3f,
            Seed = 42,
            Reasoning = reasoningOptions,
            ResponseFormat = responseFormat,
            ModelId = "gpt-4",
            StopSequences = ["STOP"],
            AllowMultipleToolCalls = true,
            ToolMode = toolMode,
            Tools = [tool]
        };

        var reasoningOptionsDto = new ReasoningOptionsDto(ReasoningEffortEnumDto.High, ReasoningOutputEnumDto.Full);
        var responseFormatDto = new ChatResponseFormatTextDto();
        var toolModeDto = new AutoChatToolModeDto();
        var toolDto = new HostedWebSearchToolDto("Search", "Search tool");

        _reasoningOptionsMapperMock
            .Setup(m => m.ToApplication(reasoningOptions))
            .Returns(reasoningOptionsDto);
        _chatResponseFormatMapperMock
            .Setup(m => m.ToApplication(responseFormat))
            .Returns(responseFormatDto);
        _chatToolModeMapperMock
            .Setup(m => m.ToApplication(toolMode))
            .Returns(toolModeDto);
        _aiToolMapperMock
            .Setup(m => m.ToApplication(tool))
            .Returns(toolDto);

        var result = _mapper.ToApplication(domain);

        Assert.Equal("conv-123", result.ConversationId);
        Assert.Equal("Test instructions", result.Instructions);
        Assert.Equal(0.7f, result.Temperature);
        Assert.Equal(1000, result.MaxOutputTokens);
        Assert.Equal(0.9f, result.TopP);
        Assert.Equal(50, result.TopK);
        Assert.Equal(0.5f, result.FrequencyPenalty);
        Assert.Equal(0.3f, result.PresencePenalty);
        Assert.Equal(42, result.Seed);
        Assert.Equal(reasoningOptionsDto, result.Reasoning);
        Assert.Equal(responseFormatDto, result.ResponseFormat);
        Assert.Equal("gpt-4", result.ModelId);
        Assert.Single(result.StopSequences!);
        Assert.Equal("STOP", result.StopSequences![0]);
        Assert.True(result.AllowMultipleToolCalls);
        Assert.Equal(toolModeDto, result.ToolMode);
        Assert.NotNull(result.Tools);
        Assert.Single(result.Tools);
        Assert.Contains(string.Empty, result.Tools.Keys);
        Assert.Single(result.Tools[string.Empty]);
        Assert.Equal(toolDto, result.Tools[string.Empty][0]);
    }

    [Fact]
    public void ToApplication_NullablePropertiesNull_MapsCorrectly()
    {
        var domain = new ChatOptions
        {
            Temperature = 1.0f,
            Reasoning = null,
            ResponseFormat = null,
            ToolMode = null,
            Tools = null
        };

        var result = _mapper.ToApplication(domain);

        Assert.Null(result.Reasoning);
        Assert.Null(result.ResponseFormat);
        Assert.Null(result.ToolMode);
        Assert.Null(result.Tools);
        Assert.Equal(1.0f, result.Temperature);
    }

    [Fact]
    public void ToApplication_EmptyToolsList_ReturnsNullToolsDictionary()
    {
        var domain = new ChatOptions
        {
            Tools = []
        };

        var result = _mapper.ToApplication(domain);

        Assert.NotNull(result.Tools);
        Assert.Empty(result.Tools[string.Empty]);
    }

    [Fact]
    public void ToDomain_AllPropertiesSet_MapsCorrectly()
    {
        var reasoningOptionsDto = new ReasoningOptionsDto(ReasoningEffortEnumDto.Medium, ReasoningOutputEnumDto.Summary);
        var responseFormatDto = new ChatResponseFormatJsonDto("{}", "schema", "desc");
        var toolModeDto = new RequiredChatToolModeDto("testFunc");
        var toolDto = new HostedWebSearchToolDto("Search", "Search tool");

        var dto = new ChatOptionsDto(
            ConversationId: "conv-456",
            Instructions: "Test instructions",
            Temperature: 0.8f,
            MaxOutputTokens: 2000,
            TopP: 0.95f,
            TopK: 40,
            FrequencyPenalty: 0.6f,
            PresencePenalty: 0.4f,
            Seed: 123,
            Reasoning: reasoningOptionsDto,
            ResponseFormat: responseFormatDto,
            ModelId: "gpt-3.5",
            StopSequences: ["END", "DONE"],
            AllowMultipleToolCalls: false,
            ToolMode: toolModeDto,
            Tools: new Dictionary<string, IList<AbstractAiToolDto>> { { "default", [toolDto] } }
        );

        var reasoningOptions = new ReasoningOptions();
        var responseFormat = new ChatResponseFormatJson(null);
        var toolMode = new RequiredChatToolMode("testFunc");
        var tool = new HostedWebSearchTool();

        _reasoningOptionsMapperMock
            .Setup(m => m.ToDomain(reasoningOptionsDto))
            .Returns(reasoningOptions);
        _chatResponseFormatMapperMock
            .Setup(m => m.ToDomain(responseFormatDto))
            .Returns(responseFormat);
        _chatToolModeMapperMock
            .Setup(m => m.ToDomain(toolModeDto))
            .Returns(toolMode);
        _aiToolMapperMock
            .Setup(m => m.ToDomain(toolDto))
            .Returns(tool);

        var result = _mapper.ToDomain(dto);

        Assert.Equal("conv-456", result.ConversationId);
        Assert.Equal("Test instructions", result.Instructions);
        Assert.Equal(0.8f, result.Temperature);
        Assert.Equal(2000, result.MaxOutputTokens);
        Assert.Equal(0.95f, result.TopP);
        Assert.Equal(40, result.TopK);
        Assert.Equal(0.6f, result.FrequencyPenalty);
        Assert.Equal(0.4f, result.PresencePenalty);
        Assert.Equal(123, result.Seed);
        Assert.Equal(reasoningOptions, result.Reasoning);
        Assert.Equal(responseFormat, result.ResponseFormat);
        Assert.Equal("gpt-3.5", result.ModelId);
        Assert.NotNull(result.StopSequences);
        Assert.Equal(2, result.StopSequences.Count);
        Assert.Contains("END", result.StopSequences);
        Assert.Contains("DONE", result.StopSequences);
        Assert.False(result.AllowMultipleToolCalls);
        Assert.Equal(toolMode, result.ToolMode);
        Assert.NotNull(result.Tools);
        Assert.Single(result.Tools);
        Assert.Equal(tool, result.Tools[0]);
    }

    [Fact]
    public void ToDomain_NullablePropertiesNull_MapsCorrectly()
    {
        var dto = new ChatOptionsDto(
            ConversationId: null,
            Instructions: null,
            Temperature: 0.5f,
            MaxOutputTokens: null,
            TopP: null,
            TopK: null,
            FrequencyPenalty: null,
            PresencePenalty: null,
            Seed: null,
            Reasoning: null,
            ResponseFormat: null,
            ModelId: null,
            StopSequences: null,
            AllowMultipleToolCalls: null,
            ToolMode: null,
            Tools: null
        );

        var result = _mapper.ToDomain(dto);

        Assert.Null(result.ConversationId);
        Assert.Null(result.Instructions);
        Assert.Equal(0.5f, result.Temperature);
        Assert.Null(result.Reasoning);
        Assert.Null(result.ResponseFormat);
        Assert.Null(result.ToolMode);
        Assert.Null(result.Tools);
    }

    [Fact]
    public void ToDomain_MultipleToolsInDictionary_FlattensToList()
    {
        var toolDto1 = new HostedWebSearchToolDto("Search1", "Tool 1");
        var toolDto2 = new HostedWebSearchToolDto("Search2", "Tool 2");
        var toolDto3 = new HostedWebSearchToolDto("Search3", "Tool 3");

        var dto = new ChatOptionsDto(
            ConversationId: null,
            Instructions: null,
            Temperature: null,
            MaxOutputTokens: null,
            TopP: null,
            TopK: null,
            FrequencyPenalty: null,
            PresencePenalty: null,
            Seed: null,
            Reasoning: null,
            ResponseFormat: null,
            ModelId: null,
            StopSequences: null,
            AllowMultipleToolCalls: null,
            ToolMode: null,
            Tools: new Dictionary<string, IList<AbstractAiToolDto>>
            {
                { "server1", [toolDto1] },
                { "server2", [toolDto2, toolDto3] }
            }
        );

        var tool1 = new HostedWebSearchTool();
        var tool2 = new HostedWebSearchTool();
        var tool3 = new HostedWebSearchTool();

        _aiToolMapperMock.Setup(m => m.ToDomain(toolDto1)).Returns(tool1);
        _aiToolMapperMock.Setup(m => m.ToDomain(toolDto2)).Returns(tool2);
        _aiToolMapperMock.Setup(m => m.ToDomain(toolDto3)).Returns(tool3);

        var result = _mapper.ToDomain(dto);

        Assert.NotNull(result.Tools);
        Assert.Equal(3, result.Tools.Count);
        Assert.Contains(tool1, result.Tools);
        Assert.Contains(tool2, result.Tools);
        Assert.Contains(tool3, result.Tools);
        _aiToolMapperMock.Verify(m => m.ToDomain(It.IsAny<AbstractAiToolDto>()), Times.Exactly(3));
    }
}
