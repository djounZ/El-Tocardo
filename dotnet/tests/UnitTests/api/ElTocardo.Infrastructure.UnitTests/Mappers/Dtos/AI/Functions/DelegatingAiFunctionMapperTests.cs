using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Functions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.Functions;

public class DelegatingAiFunctionMapperTests
{
    [Fact]
    public void ToApplication_MapsAllFields()
    {
        var loggerMock = new Mock<ILogger<DelegatingAiFunctionDeclarationMapper>>();
        var mapper = new DelegatingAiFunctionMapper(loggerMock.Object);

        // Create a test DelegatingAIFunction instance
        // Create a real AIFunctionDeclaration from a test method
        string TestMethod(string input) => input.ToUpper();
        var function = AIFunctionFactory.Create(TestMethod, name: "TestFunc", description: "Test function");

        var dto = mapper.ToApplication(new TestDelegatingAIFunction(function));


        Assert.Equal("TestFunc", dto.Name);
        Assert.Equal("Test function", dto.Description);
        Assert.NotNull(dto.Schema);
        Assert.NotNull(dto.UnderlyingMethod);
    }

    [Fact]
    public void ToDomain_ThrowsNotSupportedException()
    {
        var loggerMock = new Mock<ILogger<DelegatingAiFunctionDeclarationMapper>>();
        var mapper = new DelegatingAiFunctionMapper(loggerMock.Object);

        var dto = new DelegatingAiFunctionDto("n", "d", "s", "r", "m");

        Assert.Throws<NotSupportedException>(() => mapper.ToDomain(dto));
    }
}
