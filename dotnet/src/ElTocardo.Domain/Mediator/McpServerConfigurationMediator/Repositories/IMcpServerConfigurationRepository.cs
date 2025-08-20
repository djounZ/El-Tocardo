using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;

namespace ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;

public interface IMcpServerConfigurationRepository : IEntityRepository<McpServerConfiguration, string>;
