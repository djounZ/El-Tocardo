
using System.Text.Json;
using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core;
using FluentAssertions;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.ModelContextProtocol;

public class McpServerConfigurationItemDtoTest
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
    };

    [Fact]
    public void Constructor_AllProperties_ShouldSetProperties()
    {
        // Arrange
        var category = "test-category";
        var command = "run";
        var args = new List<string> { "a", "b" };
        var env = new Dictionary<string, string?> { { "ENV1", "VAL1" } };
        var endpoint = new Uri("http://localhost:1234");
        var type = McpServerTransportTypeDto.Http;

        // Act
        var dto = new McpServerConfigurationItemDto(category, command, args, env, endpoint, type);

        // Assert
        dto.Category.Should().Be(category);
        dto.Command.Should().Be(command);
        dto.Arguments.Should().BeEquivalentTo(args);
        dto.EnvironmentVariables.Should().BeEquivalentTo(env);
        dto.Endpoint.Should().Be(endpoint);
        dto.Type.Should().Be(type);
    }

    [Fact]
    public void Constructor_DefaultType_ShouldBeStdio()
    {
        // Act
        var dto = new McpServerConfigurationItemDto("cat", "cmd", null, null, null);

        // Assert
        dto.Type.Should().Be(McpServerTransportTypeDto.Stdio);
    }

    [Fact]
    public void Constructor_NullableProperties_ShouldAllowNulls()
    {
        // Act
        var dto = new McpServerConfigurationItemDto(null, null, null, null, null);

        // Assert
        dto.Category.Should().BeNull();
        dto.Command.Should().BeNull();
        dto.Arguments.Should().BeNull();
        dto.EnvironmentVariables.Should().BeNull();
        dto.Endpoint.Should().BeNull();
    }

    [Fact]
    public void Serialization_ShouldUseJsonPropertyNames_AndEnumAsString()
    {
        // Arrange
        var dto = new McpServerConfigurationItemDto(
            "cat", "cmd", new List<string> { "x" }, new Dictionary<string, string?> { { "K", "V" } },
            new Uri("http://host"), McpServerTransportTypeDto.Http);

        // Act
        var json = JsonSerializer.Serialize(dto, JsonOptions);

        // Assert
        json.Should().Contain("\"category\":\"cat\"");
        json.Should().Contain("\"command\":\"cmd\"");
        json.Should().Contain("\"args\":[\"x\"]");
        json.Should().Contain("\"env\":{\"K\":\"V\"}");
        json.Should().Contain("\"url\":\"http://host\"");
        json.Should().Contain("\"type\":\"http\"");
    }

    [Fact]
    public void Deserialization_ShouldSetAllProperties()
    {
        // Arrange
        var json = """
        {
            "category": "cat",
            "command": "cmd",
            "args": ["a", "b"],
            "env": { "E": "V" },
            "url": "http://localhost:42",
            "type": "stdio"
        }
        """;

        // Act
        var dto = JsonSerializer.Deserialize<McpServerConfigurationItemDto>(json, JsonOptions);

        // Assert
        dto.Should().NotBeNull();
        dto.Category.Should().Be("cat");
        dto.Command.Should().Be("cmd");
        dto.Arguments.Should().BeEquivalentTo("a", "b");
        dto.Endpoint.Should().Be(new Uri("http://localhost:42"));
        dto.Type.Should().Be(McpServerTransportTypeDto.Stdio);
    }

    [Fact]
    public void Deserialization_UnknownType_ShouldThrow()
    {
        // Arrange
        var json = """
        {
            "category": "cat",
            "type": "unknown"
        }
        """;

        // Act
        Action act = () => JsonSerializer.Deserialize<McpServerConfigurationItemDto>(json, JsonOptions);

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Equality_ShouldWorkForSameValues()
    {
        // Arrange
        var a = new McpServerConfigurationItemDto("cat", "cmd", null, null, null);
        var b = new McpServerConfigurationItemDto("cat", "cmd", null, null, null);

        // Assert
        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
