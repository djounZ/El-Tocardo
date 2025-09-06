using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Configurations;

public interface IElTocardoEntityFrameworkConfiguration
{
    IEntityTypeConfiguration<McpServerConfiguration> McpServerConfiguration { get; }
    IEntityTypeConfiguration<PresetChatInstruction> PresetChatInstruction { get; }
    IEntityTypeConfiguration<PresetChatOptions> PresetChatOptions { get; }
}
