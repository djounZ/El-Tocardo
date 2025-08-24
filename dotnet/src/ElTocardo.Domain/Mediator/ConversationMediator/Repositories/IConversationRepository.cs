using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;

namespace ElTocardo.Domain.Mediator.ConversationMediator.Repositories;

public interface IConversationRepository: IEntityRepository<IConversation,string, string>;
