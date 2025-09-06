using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Mediator.Common.Handlers.Queries;
using ElTocardo.Application.Mediator.ConversationMediator.Mappers;
using ElTocardo.Application.Mediator.ConversationMediator.Queries;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.ConversationMediator.Handlers.Queries;

public class GetAllConversationsQueryHandler(
    IConversationRepository repository,
    ILogger<GetAllConversationsQueryHandler> logger,
    ConversationDomainGetAllDtoMapper mapper)
    : GetAllEntitiesQueryHandler<Conversation, string, string, GetAllConversationsQuery,ConversationDto[]>(repository, logger, mapper);


public class GetAllConversationSummariesQueryHandler(
    IConversationRepository repository,
    ILogger<GetAllConversationSummariesQueryHandler> logger,
    ConversationDomainGetAllSummariesDtoMapper mapper)
    : GetAllEntitiesQueryHandler<Conversation, string, string, GetAllConversationsQuery,ConversationSummaryDto[]>(repository, logger, mapper);
