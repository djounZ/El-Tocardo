using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using ElTocardo.Infrastructure.Mediator.MongoDb.Data;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ElTocardo.Infrastructure.Mediator.MongoDb.Repositories;

public class ConversationRepository(ILogger<ConversationRepository> logger, IMongoCollection<ConversationMongoDb> conversationCollection) : IConversationRepository
{
    public Task<IEnumerable<IConversation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all conversations");
        throw new NotImplementedException();
    }

    public Task<IConversation?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IConversation?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var asyncCursor = await conversationCollection.FindAsync(x => x.Title == key, cancellationToken: cancellationToken);
        return await asyncCursor.AnyAsync(cancellationToken);
    }

    public Task AddAsync(IConversation configuration, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(IConversation configuration, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(IConversation configuration, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
