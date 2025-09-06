using ElTocardo.Domain.Mediator.Common.Queries;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;

public sealed record GetMcpServerByNameQuery(string Key) : GetByKeyQueryBase<string>(Key);
