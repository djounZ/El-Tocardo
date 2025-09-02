using System.Collections;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Configurations;

public class ElTocardoEntityFrameworkConfigurationEnumerator(IElTocardoEntityFrameworkConfiguration entityFrameworkConfiguration) : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        yield return entityFrameworkConfiguration.McpServerConfiguration;
        yield return entityFrameworkConfiguration.PresetChatInstruction;
        yield return entityFrameworkConfiguration.PresetChatOptions;
    }
}