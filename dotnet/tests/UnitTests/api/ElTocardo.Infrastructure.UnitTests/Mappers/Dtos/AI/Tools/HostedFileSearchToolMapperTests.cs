using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Tools;
using ElTocardo.Application.Mappers.Dtos;
using Microsoft.Extensions.AI;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.Tools;

public class HostedFileSearchToolMapperTests
{
    private readonly Mock<IDomainEntityMapper<AIContent, AiContentDto>> _aiContentMapperMock;
    private readonly IDomainEntityMapper<HostedFileSearchTool, HostedFileSearchToolDto> _mapper;

    public HostedFileSearchToolMapperTests()
    {
        _aiContentMapperMock = new Mock<IDomainEntityMapper<AIContent, AiContentDto>>();
        _mapper = new HostedFileSearchToolMapper(_aiContentMapperMock.Object);
    }

    [Fact]
    public void ToApplication_NullInputs_ReturnsDtoWithEmptyInputs()
    {
        var domain = new HostedFileSearchTool();

        var result = _mapper.ToApplication(domain);

        Assert.NotNull(result);
        Assert.NotNull(result.Inputs);
        Assert.Empty(result.Inputs);
        _aiContentMapperMock.Verify(m => m.ToApplication(It.IsAny<AIContent>()), Times.Never);
    }

    [Fact]
    public void ToDomain_WithInputs_MapsInputsAndMaximumResultCount()
    {
        var contentDto = new TextContentDto(null, "query");
        var dto = new HostedFileSearchToolDto(
            Name: "FileSearch",
            Description: "Search files",
            Inputs: new List<AiContentDto> { contentDto },
            MaximumResultCount: 7
        );

        var contentDomain = new TextContent("query");
        _aiContentMapperMock.Setup(m => m.ToDomain(contentDto)).Returns(contentDomain);

        var result = _mapper.ToDomain(dto);

        Assert.NotNull(result);
        Assert.NotNull(result.Inputs);
        Assert.Single(result.Inputs);
        Assert.Equal(contentDomain, result.Inputs[0]);
        Assert.Equal(7, result.MaximumResultCount);
        _aiContentMapperMock.Verify(m => m.ToDomain(contentDto), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void ToDomain_MaximumResultCount_IsPreserved(int max)
    {
        var dto = new HostedFileSearchToolDto(
            Name: "S",
            Description: "D",
            Inputs: new List<AiContentDto>(),
            MaximumResultCount: max
        );

        var result = _mapper.ToDomain(dto);

        Assert.Equal(max, result.MaximumResultCount);
    }
}
