using ElTocardo.Application.Dtos.Microsoft.Extensions.AI;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;
using Microsoft.Extensions.AI;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI;

public class UsageDetailsMapperTests
{
    [Fact]
    public void ToApplication_AllPropertiesSet_MapsCorrectly()
    {
        var mapper = new UsageDetailsMapper();

        var domain = new UsageDetails
        {
            InputTokenCount = 10,
            OutputTokenCount = 20,
            TotalTokenCount = 30,
            CachedInputTokenCount = 5,
            ReasoningTokenCount = 2,
            AdditionalCounts = new AdditionalPropertiesDictionary<long>
            {
                ["extra"] = 7
            }
        };

        var dto = mapper.ToApplication(domain);

        Assert.Equal(10, dto.InputTokenCount);
        Assert.Equal(20, dto.OutputTokenCount);
        Assert.Equal(30, dto.TotalTokenCount);
        Assert.Equal(5, dto.CachedInputTokenCount);
        Assert.Equal(2, dto.ReasoningTokenCount);
        Assert.NotNull(dto.AdditionalCounts);
        Assert.Equal(7, dto.AdditionalCounts!["extra"]);
    }

    [Fact]
    public void ToApplication_NullAdditionalCounts_MapsCorrectly()
    {
        var mapper = new UsageDetailsMapper();

        var domain = new UsageDetails
        {
            InputTokenCount = 1,
            OutputTokenCount = 2,
            TotalTokenCount = 3,
            CachedInputTokenCount = null,
            ReasoningTokenCount = null,
            AdditionalCounts = null
        };

        var dto = mapper.ToApplication(domain);

        Assert.Equal(1, dto.InputTokenCount);
        Assert.Equal(2, dto.OutputTokenCount);
        Assert.Equal(3, dto.TotalTokenCount);
        Assert.Null(dto.CachedInputTokenCount);
        Assert.Null(dto.ReasoningTokenCount);
        Assert.Null(dto.AdditionalCounts);
    }

    [Fact]
    public void ToDomain_AllPropertiesSet_MapsCorrectly()
    {
        var mapper = new UsageDetailsMapper();

        var dto = new UsageDetailsDto(
            InputTokenCount: 11,
            OutputTokenCount: 22,
            TotalTokenCount: 33,
            CachedInputTokenCount: 6,
            ReasoningTokenCount: 3,
            AdditionalCounts: new Dictionary<string, long> { ["a"] = 9 }
        );

        var domain = mapper.ToDomain(dto);

        Assert.Equal(11, domain.InputTokenCount);
        Assert.Equal(22, domain.OutputTokenCount);
        Assert.Equal(33, domain.TotalTokenCount);
        Assert.Equal(6, domain.CachedInputTokenCount);
        Assert.Equal(3, domain.ReasoningTokenCount);
        // mapper does not populate AdditionalCounts into domain object
        Assert.True(domain.AdditionalCounts == null || domain.AdditionalCounts.Count == 0);
    }

    [Fact]
    public void ToDomain_Nullables_NullMappedCorrectly()
    {
        var mapper = new UsageDetailsMapper();

        var dto = new UsageDetailsDto(
            InputTokenCount: null,
            OutputTokenCount: null,
            TotalTokenCount: null,
            CachedInputTokenCount: null,
            ReasoningTokenCount: null,
            AdditionalCounts: null
        );

        var domain = mapper.ToDomain(dto);

        Assert.Null(domain.InputTokenCount);
        Assert.Null(domain.OutputTokenCount);
        Assert.Null(domain.TotalTokenCount);
        Assert.Null(domain.CachedInputTokenCount);
        Assert.Null(domain.ReasoningTokenCount);
    }
}
