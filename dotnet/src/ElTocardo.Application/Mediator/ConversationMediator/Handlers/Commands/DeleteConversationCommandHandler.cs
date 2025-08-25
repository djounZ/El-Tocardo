using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.ConversationMediator.Handlers.Commands;

public class DeleteConversationCommandHandler(
    IConversationRepository repository,
    ILogger<DeleteConversationCommandHandler> logger)
    : DeleteEntityCommandHandler<Conversation, string, string, DeleteConversationCommand>(repository, logger);
