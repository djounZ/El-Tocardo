using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.AI;

namespace ElTocardo.Domain.Mediator.ConversationMediator.Repositories;

public interface IConversationRepository : IEntityRepository<Conversation, string, string>
{

    public Task<Result<Conversation>> AddRoundAsync(string id, ConversationRound conversationRound, CancellationToken cancellationToken = default);
    public Task<Result<Conversation>> UpdateRoundAsync(string id, ChatResponse chatResponse, CancellationToken cancellationToken = default);


}
