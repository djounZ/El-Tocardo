using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Tools;
using ElTocardo.Application.Mappers.Dtos;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.Tools;

public class AIToolMapperTests
{
    private readonly Mock<ILogger<AIToolMapper>> _loggerMock;
    private readonly Mock<IDomainEntityMapper<HostedFileSearchTool, HostedFileSearchToolDto>> _hostedFileMapperMock;
    private readonly Mock<IDomainEntityMapper<HostedWebSearchTool, HostedWebSearchToolDto>> _hostedWebMapperMock;
    private readonly Mock<IDomainEntityMapper<AIFunctionDeclaration, DelegatingAiFunctionDeclarationDto>> _functionDeclMapperMock;
    private readonly Mock<IDomainEntityMapper<DelegatingAIFunction, DelegatingAiFunctionDto>> _functionMapperMock;
    private readonly AIToolMapper _mapper;

    public AIToolMapperTests()
    {
        _loggerMock = new Mock<ILogger<AIToolMapper>>();
        _hostedFileMapperMock = new Mock<IDomainEntityMapper<HostedFileSearchTool, HostedFileSearchToolDto>>();
        _hostedWebMapperMock = new Mock<IDomainEntityMapper<HostedWebSearchTool, HostedWebSearchToolDto>>();
        _functionDeclMapperMock = new Mock<IDomainEntityMapper<AIFunctionDeclaration, DelegatingAiFunctionDeclarationDto>>();
        _functionMapperMock = new Mock<IDomainEntityMapper<DelegatingAIFunction, DelegatingAiFunctionDto>>();

        _mapper = new AIToolMapper(
            _loggerMock.Object,
            _hostedFileMapperMock.Object,
            _hostedWebMapperMock.Object,
            _functionDeclMapperMock.Object,
            _functionMapperMock.Object
        );
    }

    [Fact]
    public void ToApplication_HostedFileSearchTool_DelegatesToMapper()
    {
        var domain = new HostedFileSearchTool();
        var dto = new HostedFileSearchToolDto("n", "d", new List<AiContentDto>(), 0);

        _hostedFileMapperMock.Setup(m => m.ToApplication(domain)).Returns(dto);

        var result = _mapper.ToApplication(domain);

        Assert.Equal(dto, result);
        _hostedFileMapperMock.Verify(m => m.ToApplication(domain), Times.Once);
    }

    [Fact]
    public void ToApplication_HostedWebSearchTool_DelegatesToMapper()
    {
        var domain = new HostedWebSearchTool();
        var dto = new HostedWebSearchToolDto("n", "d");

        _hostedWebMapperMock.Setup(m => m.ToApplication(domain)).Returns(dto);

        var result = _mapper.ToApplication(domain);

        Assert.Equal(dto, result);
        _hostedWebMapperMock.Verify(m => m.ToApplication(domain), Times.Once);
    }

    [Fact]
    public void ToDomain_HostedFileSearchToolDto_DelegatesToMapper()
    {
        var dto = new HostedFileSearchToolDto("n", "d", new List<AiContentDto>(), 3);
        var domain = new HostedFileSearchTool();

        _hostedFileMapperMock.Setup(m => m.ToDomain(dto)).Returns(domain);

        var result = _mapper.ToDomain(dto);

        Assert.Equal(domain, result);
        _hostedFileMapperMock.Verify(m => m.ToDomain(dto), Times.Once);
    }

    [Fact]
    public void ToDomain_AiToolDto_ThrowsNotSupported()
    {
        var dto = new AiToolDto("x", "y");

        Assert.Throws<NotSupportedException>(() => _mapper.ToDomain(dto));
    }
}
