using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Infrastructure.Mediator.Data;
using ElTocardo.Infrastructure.Mediator.Repositories.Common;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.Repositories;

public class McpServerConfigurationRepository(
    ApplicationDbContext context,
    ILogger<McpServerConfigurationRepository> logger)
    : EntityRepository<McpServerConfiguration, string>(context, context.McpServerConfigurations, logger), IMcpServerConfigurationRepository;
