using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Tools;
using Microsoft.Extensions.AI;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.Tools;

public class HostedWebSearchToolMapperTests
{
    private readonly IDomainEntityMapper<HostedWebSearchTool, HostedWebSearchToolDto> _mapper;

    public HostedWebSearchToolMapperTests()
    {
        _mapper = new HostedWebSearchToolMapper();
    }

    [Fact]
    public void ToApplication_ValidDomain_ReturnsDto()
    {
        var domain = new HostedWebSearchTool();

        var result = _mapper.ToApplication(domain);

        Assert.NotNull(result);
        Assert.IsType<HostedWebSearchToolDto>(result);
    }

    [Fact]
    public void ToApplication_ValidDomain_MapsNameAndDescription()
    {
        var domain = new HostedWebSearchTool();

        var result = _mapper.ToApplication(domain);

        Assert.NotNull(result);
        Assert.Equal(domain.Name, result.Name);
        Assert.Equal(domain.Description, result.Description);
    }

    [Fact]
    public void ToDomain_AllPropertiesSet_ReturnsNewInstance()
    {
        var dto = new HostedWebSearchToolDto(
            Name: "WebSearch",
            Description: "Search the web"
        );

        var result = _mapper.ToDomain(dto);

        Assert.NotNull(result);
        Assert.IsType<HostedWebSearchTool>(result);
    }

    [Fact]
    public void ToDomain_EmptyProperties_ReturnsNewInstance()
    {
        var dto = new HostedWebSearchToolDto(
            Name: "",
            Description: ""
        );

        var result = _mapper.ToDomain(dto);

        Assert.NotNull(result);
        Assert.IsType<HostedWebSearchTool>(result);
    }

    [Theory]
    [InlineData("Tool1", "Description 1")]
    [InlineData("Tool2", "Description 2")]
    [InlineData("WebSearchAPI", "Searches web content")]
    [InlineData("", "")]
    public void ToDomain_VariousInputs_ReturnsNewInstance(string name, string description)
    {
        var dto = new HostedWebSearchToolDto(
            Name: name,
            Description: description
        );

        var result = _mapper.ToDomain(dto);

        Assert.NotNull(result);
        Assert.IsType<HostedWebSearchTool>(result);
    }

    [Fact]
    public void ToApplication_MultipleCalls_ReturnsConsistentResults()
    {
        var domain = new HostedWebSearchTool();

        var result1 = _mapper.ToApplication(domain);
        var result2 = _mapper.ToApplication(domain);

        Assert.Equal(result1.Name, result2.Name);
        Assert.Equal(result1.Description, result2.Description);
    }

    [Fact]
    public void ToDomain_MultipleCalls_ReturnsNewInstances()
    {
        var dto = new HostedWebSearchToolDto(
            Name: "SearchTool",
            Description: "Tool description"
        );

        var result1 = _mapper.ToDomain(dto);
        var result2 = _mapper.ToDomain(dto);

        Assert.NotSame(result1, result2);
        Assert.IsType<HostedWebSearchTool>(result1);
        Assert.IsType<HostedWebSearchTool>(result2);
    }
}
