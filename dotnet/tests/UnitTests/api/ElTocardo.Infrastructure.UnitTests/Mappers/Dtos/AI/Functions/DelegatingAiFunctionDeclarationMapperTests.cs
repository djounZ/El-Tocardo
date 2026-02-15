using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Functions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.Functions;

public class DelegatingAiFunctionDeclarationMapperTests
{
    [Fact]
    public void ToApplication_MapsPropertiesCorrectly()
    {
        var loggerMock = new Mock<ILogger<DelegatingAiFunctionDeclarationMapper>>();
        var mapper = new DelegatingAiFunctionDeclarationMapper(loggerMock.Object);

        // Create a real AIFunctionDeclaration from a test method
        string TestMethod(string input) => input.ToUpper();
        var function = AIFunctionFactory.Create(TestMethod, name: "MyFunc", description: "Does stuff");

        var dto = mapper.ToApplication(new TestDelegatingAIFunction(function));

        Assert.Equal("MyFunc", dto.Name);
        Assert.Equal("Does stuff", dto.Description);
        Assert.NotNull(dto.Schema);
        Assert.NotNull(dto.ReturnJsonSchema);
    }

    [Fact]
    public void ToDomain_ThrowsNotSupportedException()
    {
        var loggerMock = new Mock<ILogger<DelegatingAiFunctionDeclarationMapper>>();
        var mapper = new DelegatingAiFunctionDeclarationMapper(loggerMock.Object);

        var dto = new DelegatingAiFunctionDeclarationDto("n", "d", "s", "r");

        Assert.Throws<NotSupportedException>(() => mapper.ToDomain(dto));
    }
}
