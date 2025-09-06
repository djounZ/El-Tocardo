using ElTocardo.Domain.Mediator.Commands;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;

public sealed record DeleteMcpServerCommand(string Key) : DeleteCommandBase<string>(Key);
