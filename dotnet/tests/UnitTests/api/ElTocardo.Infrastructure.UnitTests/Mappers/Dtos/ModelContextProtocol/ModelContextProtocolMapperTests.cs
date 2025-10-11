using System.Text.Json;
using System.Text.Json.Nodes;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mappers.Dtos.ModelContextProtocol;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.ModelContextProtocol;

public class ModelContextProtocolMapperTests
{
    private readonly Mock<ILogger<ModelContextProtocolMapper>> _loggerMock = new();
    private readonly ModelContextProtocolMapper _mapper;

    public ModelContextProtocolMapperTests()
    {
        _mapper = new ModelContextProtocolMapper(_loggerMock.Object);
    }

    // Helper: Minimal Tool and McpClientTool
    private static McpClientTool CreateMcpClientTool(string name = "tool1", string? desc = "desc")
    {
        var tool = new Tool
        {
            Name = name,
            Description = desc,
            InputSchema = JsonDocument.Parse("{}").RootElement,
            OutputSchema = JsonDocument.Parse("{}").RootElement
        };
        var client = new Mock<McpClient>().Object;
        return (McpClientTool)Activator.CreateInstance(typeof(McpClientTool), true)!
            .GetType()
            .GetConstructor([
                typeof(McpClient), typeof(Tool), typeof(JsonSerializerOptions), typeof(string), typeof(string),
                typeof(IProgress<ProgressNotificationValue>)
            ])!
            .Invoke([client, tool, new JsonSerializerOptions(), name, desc, null]);
    }

    // Helper: Minimal Prompt and McpClientPrompt
    private static McpClientPrompt CreateMcpClientPrompt(string name = "prompt1", string? desc = "desc",
        IList<PromptArgument>? args = null, JsonObject? meta = null)
    {
        var prompt = new Prompt
        {
            Name = name,
            Title = "title",
            Description = desc,
            Arguments = args ?? new List<PromptArgument>(),
            Meta = meta
        };
        var client = new Mock<McpClient>().Object;
        return (McpClientPrompt)Activator.CreateInstance(typeof(McpClientPrompt), true)!
            .GetType()
            .GetConstructor([typeof(McpClient), typeof(Prompt)])!
            .Invoke([client, prompt]);
    }

    [Fact]
    public void MapToMcpClientToolDtos_Empty_ReturnsEmptyArray()
    {
        var dtos = _mapper.MapToMcpClientToolDtos([]);
        Assert.Empty(dtos);
    }

    [Fact]
    public void MapToMcpClientPromptDtos_Empty_ReturnsEmptyArray()
    {
        var dtos = _mapper.MapToMcpClientPromptDtos([]);
        Assert.Empty(dtos);
    }

    // Helper: Minimal ResourceTemplate and McpClientResourceTemplate
    private static McpClientResourceTemplate CreateMcpClientResourceTemplate(string name = "rt1", string? desc = "desc",
        JsonObject? meta = null, Annotations? annotations = null)
    {
        var proto = new ResourceTemplate
        {
            Name = name,
            Title = "title",
            UriTemplate = "uri/{id}",
            Description = desc,
            MimeType = "text/plain",
            Meta = meta,
            Annotations = annotations
        };
        var client = new Mock<McpClient>().Object;
        return (McpClientResourceTemplate)Activator.CreateInstance(typeof(McpClientResourceTemplate), true)!
            .GetType()
            .GetConstructor([typeof(McpClient), typeof(ResourceTemplate)])!
            .Invoke([client, proto]);
    }

    // Helper: Minimal Resource and McpClientResource
    private static McpClientResource CreateMcpClientResource(string name = "res1", string? desc = "desc",
        JsonObject? meta = null, Annotations? annotations = null, long? size = 1)
    {
        var proto = new Resource
        {
            Name = name,
            Title = "title",
            Uri = "uri/1",
            Description = desc,
            MimeType = "text/plain",
            Meta = meta,
            Annotations = annotations,
            Size = size
        };
        var client = new Mock<McpClient>().Object;
        return (McpClientResource)Activator.CreateInstance(typeof(McpClientResource), true)!
            .GetType()
            .GetConstructor([typeof(McpClient), typeof(Resource)])!
            .Invoke([client, proto]);
    }

    [Fact]
    public void MapToMcpClientResourceTemplateDtos_Empty_ReturnsEmptyArray()
    {
        var dtos = _mapper.MapToMcpClientResourceTemplateDtos([]);
        Assert.Empty(dtos);
    }

    [Fact]
    public void MapToMcpClientResourceDtos_Empty_ReturnsEmptyArray()
    {
        var dtos = _mapper.MapToMcpClientResourceDtos([]);
        Assert.Empty(dtos);
    }

    // Helpers for ContentBlock and CallToolResult
    private static TextContentBlock CreateTextBlock(string text = "hi")
    {
        return new TextContentBlock { Text = text };
    }

    private static ImageContentBlock CreateImageBlock(string? data = null)
    {
        return new ImageContentBlock
        {
            Data = data ?? Convert.ToBase64String(new byte[] { 1, 2 }), MimeType = "image/png"
        };
    }

    private static AudioContentBlock CreateAudioBlock(string? data = null)
    {
        return new AudioContentBlock
        {
            Data = data ?? Convert.ToBase64String(new byte[] { 3, 4 }), MimeType = "audio/wav"
        };
    }

    private static EmbeddedResourceBlock CreateEmbeddedResourceBlock(ResourceContents? resource = null)
    {
        return new EmbeddedResourceBlock
        {
            Resource = resource ?? new BlobResourceContents
            {
                Uri = "uri", MimeType = "m", Blob = Convert.ToBase64String(new byte[] { 5 }), Meta = null
            }
        };
    }

    private static ResourceLinkBlock CreateResourceLinkBlock()
    {
        return new ResourceLinkBlock
        {
            Uri = "uri",
            Name = "n",
            Description = "d",
            MimeType = "m",
            Size = 1
        };
    }

    [Fact]
    public void MapToCallToolResult_MapsAllContentBlockTypes()
    {
        var blocks = new ContentBlock[]
        {
            CreateTextBlock("t"), CreateImageBlock(), CreateAudioBlock(), CreateEmbeddedResourceBlock(),
            CreateResourceLinkBlock()
        };
        var result = new CallToolResult { Content = blocks, StructuredContent = null, IsError = false };
        var dto = _mapper.MapToCallToolResultDto(result);
        Assert.Equal(5, dto.Content.Count);
        Assert.Contains(dto.Content, b => b is TextContentBlockDto);
        Assert.Contains(dto.Content, b => b is ImageContentBlockDto);
        Assert.Contains(dto.Content, b => b is AudioContentBlockDto);
        Assert.Contains(dto.Content, b => b is EmbeddedResourceBlockDto);
        Assert.Contains(dto.Content, b => b is ResourceLinkBlockDto);
    }

    [Fact]
    public void MapToStdioClientTransportOptions_MapsAllFields()
    {
        var dto = new McpServerConfigurationItemDto(
            null, // Category
            "cmd", // Command
            ["a1", "a2"], // Arguments
            new Dictionary<string, string?> { ["k"] = "v" }, // Env
            null // Type
        );
        var opt = _mapper.MapToStdioClientTransportOptions(dto);
        Assert.Equal("cmd", opt.Command);
        Assert.Equal(["a1", "a2"], opt.Arguments);
        Assert.Equal("v", opt.EnvironmentVariables!["k"]);
    }

    [Fact]
    public void MapToSseClientTransportOptions_MapsAllFields()
    {
        var uri = new Uri("http://localhost");
        var dto = new McpServerConfigurationItemDto(
            null, // Category
            null, // Command
            null, // Arguments
            null, // Env
            uri, // Endpoint
            McpServerTransportTypeDto.Http // Type (used for SSE/HTTP)
        );
        var opt = _mapper.MapToHttpClientTransportOptions(dto);
        Assert.Equal(uri, opt.Endpoint);
    }
}
