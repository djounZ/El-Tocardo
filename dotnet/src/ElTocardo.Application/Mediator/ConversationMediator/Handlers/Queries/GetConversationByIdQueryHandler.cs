using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Mediator.Common.Handlers.Queries;
using ElTocardo.Application.Mediator.ConversationMediator.Mappers;
using ElTocardo.Application.Mediator.ConversationMediator.Queries;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.ConversationMediator.Handlers.Queries;

public class GetConversationByIdQueryHandler(
    IConversationRepository repository,
    ILogger<GetConversationByIdQueryHandler> logger,
    ConversationDomainGetDtoMapper mapper)
    : GetEntityByIdQueryHandler<Conversation, string, string, GetConversationByIdQuery, ConversationResponseDto>(repository, logger, mapper);
