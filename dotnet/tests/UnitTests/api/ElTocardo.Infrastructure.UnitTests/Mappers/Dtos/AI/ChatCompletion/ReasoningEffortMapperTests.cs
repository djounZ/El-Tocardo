using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ReasoningEffortMapperTests
{
    private static readonly Mock<ILogger<ReasoningEffortMapper>> LoggerMock = new();
    private readonly IDomainEntityMapper<ReasoningEffort, ReasoningEffortEnumDto> _mapper = new ReasoningEffortMapper(LoggerMock.Object);

    [Theory]
    [InlineData(ReasoningEffort.None, ReasoningEffortEnumDto.None)]
    [InlineData(ReasoningEffort.Low, ReasoningEffortEnumDto.Low)]
    [InlineData(ReasoningEffort.Medium, ReasoningEffortEnumDto.Medium)]
    [InlineData(ReasoningEffort.High, ReasoningEffortEnumDto.High)]
    [InlineData(ReasoningEffort.ExtraHigh, ReasoningEffortEnumDto.ExtraHigh)]
    public void ToApplication_KnownValue_MapsCorrectly(ReasoningEffort domain, ReasoningEffortEnumDto expectedDto)
    {
        var result = _mapper.ToApplication(domain);

        Assert.Equal(expectedDto, result);
    }

    [Theory]
    [InlineData(ReasoningEffortEnumDto.None, ReasoningEffort.None)]
    [InlineData(ReasoningEffortEnumDto.Low, ReasoningEffort.Low)]
    [InlineData(ReasoningEffortEnumDto.Medium, ReasoningEffort.Medium)]
    [InlineData(ReasoningEffortEnumDto.High, ReasoningEffort.High)]
    [InlineData(ReasoningEffortEnumDto.ExtraHigh, ReasoningEffort.ExtraHigh)]
    public void ToDomain_KnownValue_MapsCorrectly(ReasoningEffortEnumDto dto, ReasoningEffort expectedDomain)
    {
        var result = _mapper.ToDomain(dto);

        Assert.Equal(expectedDomain, result);
    }

    [Fact]
    public void ToApplication_UnknownValue_ThrowsArgumentOutOfRangeException()
    {
        var unknown = (ReasoningEffort)999;

        Assert.Throws<ArgumentOutOfRangeException>(() => _mapper.ToApplication(unknown));
    }

    [Fact]
    public void ToDomain_UnknownValue_ThrowsArgumentOutOfRangeException()
    {
        var unknown = (ReasoningEffortEnumDto)999;

        Assert.Throws<ArgumentOutOfRangeException>(() => _mapper.ToDomain(unknown));
    }
}
