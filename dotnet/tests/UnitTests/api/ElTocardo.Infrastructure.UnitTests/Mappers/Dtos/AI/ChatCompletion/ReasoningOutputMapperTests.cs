using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.ChatCompletion;

public class ReasoningOutputMapperTests
{
    private static readonly Mock<ILogger<ReasoningOutputMapper>> LoggerMock = new();
    private readonly IDomainEntityMapper<ReasoningOutput, ReasoningOutputEnumDto> _mapper = new ReasoningOutputMapper(LoggerMock.Object);

    [Theory]
    [InlineData(ReasoningOutput.None, ReasoningOutputEnumDto.None)]
    [InlineData(ReasoningOutput.Summary, ReasoningOutputEnumDto.Summary)]
    [InlineData(ReasoningOutput.Full, ReasoningOutputEnumDto.Full)]
    public void ToApplication_KnownValue_MapsCorrectly(ReasoningOutput domain, ReasoningOutputEnumDto expectedDto)
    {
        var result = _mapper.ToApplication(domain);

        Assert.Equal(expectedDto, result);
    }

    [Theory]
    [InlineData(ReasoningOutputEnumDto.None, ReasoningOutput.None)]
    [InlineData(ReasoningOutputEnumDto.Summary, ReasoningOutput.Summary)]
    [InlineData(ReasoningOutputEnumDto.Full, ReasoningOutput.Full)]
    public void ToDomain_KnownValue_MapsCorrectly(ReasoningOutputEnumDto dto, ReasoningOutput expectedDomain)
    {
        var result = _mapper.ToDomain(dto);

        Assert.Equal(expectedDomain, result);
    }

    [Fact]
    public void ToApplication_UnknownValue_ThrowsArgumentOutOfRangeException()
    {
        var unknown = (ReasoningOutput)999;

        Assert.Throws<ArgumentOutOfRangeException>(() => _mapper.ToApplication(unknown));
    }

    [Fact]
    public void ToDomain_UnknownValue_ThrowsArgumentOutOfRangeException()
    {
        var unknown = (ReasoningOutputEnumDto)999;

        Assert.Throws<ArgumentOutOfRangeException>(() => _mapper.ToDomain(unknown));
    }
}
