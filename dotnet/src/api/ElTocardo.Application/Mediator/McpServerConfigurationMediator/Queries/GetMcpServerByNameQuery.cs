using ElTocardo.Domain.Mediator.Queries;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;

public sealed record GetMcpServerByNameQuery(string Key) : GetByKeyQueryBase<string>(Key);
