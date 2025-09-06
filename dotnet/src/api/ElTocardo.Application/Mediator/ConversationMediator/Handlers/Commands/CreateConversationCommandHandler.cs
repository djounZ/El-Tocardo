using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Application.Mediator.ConversationMediator.Mappers;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.ConversationMediator.Handlers.Commands;

public class CreateConversationCommandHandler(
    IConversationRepository repository,
    ILogger<CreateConversationCommandHandler> logger,
    IValidator<CreateConversationCommand> validator,
    ConversationDomainCreateCommandMapper createCommandMapper)
    : CreateEntityCommandHandler<Conversation, string, string, CreateConversationCommand>(repository, logger, validator, createCommandMapper);
