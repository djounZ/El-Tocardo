using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ReasoningOptionsMapperTests
{
    private readonly Mock<IDomainEntityMapper<ReasoningEffort, ReasoningEffortEnumDto>> _reasoningEffortMapperMock;
    private readonly Mock<IDomainEntityMapper<ReasoningOutput, ReasoningOutputEnumDto>> _reasoningOutputMapperMock;
    private readonly IDomainEntityMapper<ReasoningOptions, ReasoningOptionsDto> _mapper;

    public ReasoningOptionsMapperTests()
    {
        _reasoningEffortMapperMock = new Mock<IDomainEntityMapper<ReasoningEffort, ReasoningEffortEnumDto>>();
        _reasoningOutputMapperMock = new Mock<IDomainEntityMapper<ReasoningOutput, ReasoningOutputEnumDto>>();
        _mapper = new ReasoningOptionsMapper(_reasoningEffortMapperMock.Object, _reasoningOutputMapperMock.Object);
    }

    [Fact]
    public void ToApplication_ValidReasoningOptions_MapsCorrectly()
    {
        var domain = new ReasoningOptions
        {
            Effort = ReasoningEffort.High,
            Output = ReasoningOutput.Full
        };

        _reasoningEffortMapperMock
            .Setup(m => m.ToApplication(ReasoningEffort.High))
            .Returns(ReasoningEffortEnumDto.High);
        _reasoningOutputMapperMock
            .Setup(m => m.ToApplication(ReasoningOutput.Full))
            .Returns(ReasoningOutputEnumDto.Full);

        var result = _mapper.ToApplication(domain);

        Assert.Equal(ReasoningEffortEnumDto.High, result.Effort);
        Assert.Equal(ReasoningOutputEnumDto.Full, result.Output);
        _reasoningEffortMapperMock.Verify(m => m.ToApplication(ReasoningEffort.High), Times.Once);
        _reasoningOutputMapperMock.Verify(m => m.ToApplication(ReasoningOutput.Full), Times.Once);
    }

    [Fact]
    public void ToDomain_ValidReasoningOptionsDto_MapsCorrectly()
    {
        var dto = new ReasoningOptionsDto(ReasoningEffortEnumDto.Medium, ReasoningOutputEnumDto.Summary);

        _reasoningEffortMapperMock
            .Setup(m => m.ToDomain(ReasoningEffortEnumDto.Medium))
            .Returns(ReasoningEffort.Medium);
        _reasoningOutputMapperMock
            .Setup(m => m.ToDomain(ReasoningOutputEnumDto.Summary))
            .Returns(ReasoningOutput.Summary);

        var result = _mapper.ToDomain(dto);

        Assert.Equal(ReasoningEffort.Medium, result.Effort);
        Assert.Equal(ReasoningOutput.Summary, result.Output);
        _reasoningEffortMapperMock.Verify(m => m.ToDomain(ReasoningEffortEnumDto.Medium), Times.Once);
        _reasoningOutputMapperMock.Verify(m => m.ToDomain(ReasoningOutputEnumDto.Summary), Times.Once);
    }
}
