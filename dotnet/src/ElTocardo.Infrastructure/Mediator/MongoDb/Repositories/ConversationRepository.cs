using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ElTocardo.Infrastructure.Mediator.MongoDb.Repositories;

public class ConversationRepository(ILogger<ConversationRepository> logger, IMongoCollection<Conversation> conversationCollection) : IConversationRepository
{
    public async Task<IEnumerable<Conversation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting all conversations");
        var conversations = await conversationCollection.Find(_ => true).ToListAsync(cancellationToken);
        logger.LogDebug("Retrieved {Count} conversations", conversations.Count);
        return conversations;
    }

    public async Task<Conversation?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting conversation by key: {Key}", key);
        var conversation = await conversationCollection.Find(c => c.Title == key).FirstOrDefaultAsync(cancellationToken);
        logger.LogDebug("Found conversation: {Found}", conversation != null);
        return conversation;
    }

    public async Task<Conversation?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting conversation by ID: {Id}", id);
        var conversation = await conversationCollection.Find(c => c.Id == id).FirstOrDefaultAsync(cancellationToken);
        logger.LogDebug("Found conversation: {Found}", conversation != null);
        return conversation;
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Checking if conversation exists with key: {Key}", key);
        var count = await conversationCollection.CountDocumentsAsync(c => c.Title == key, cancellationToken: cancellationToken);
        var exists = count > 0;
        logger.LogDebug("Conversation exists: {Exists}", exists);
        return exists;
    }

    public async Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Adding conversation with ID: {Id}", conversation.Id);
        await conversationCollection.InsertOneAsync(conversation, cancellationToken: cancellationToken);
        logger.LogDebug("Successfully added conversation with ID: {Id}", conversation.Id);
    }

    public async Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Updating conversation with ID: {Id}", conversation.Id);
        var filter = Builders<Conversation>.Filter.Eq(c => c.Id, conversation.Id);
        var result = await conversationCollection.ReplaceOneAsync(filter, conversation, cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
        {
            logger.LogWarning("No conversation found with ID: {Id} for update", conversation.Id);
            throw new InvalidOperationException($"Conversation with ID {conversation.Id} not found for update");
        }

        logger.LogDebug("Successfully updated conversation with ID: {Id}", conversation.Id);
    }

    public async Task DeleteAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Deleting conversation with ID: {Id}", conversation.Id);
        var filter = Builders<Conversation>.Filter.Eq(c => c.Id, conversation.Id);
        var result = await conversationCollection.DeleteOneAsync(filter, cancellationToken);

        if (result.DeletedCount == 0)
        {
            logger.LogWarning("No conversation found with ID: {Id} for deletion", conversation.Id);
            throw new InvalidOperationException($"Conversation with ID {conversation.Id} not found for deletion");
        }

        logger.LogDebug("Successfully deleted conversation with ID: {Id}", conversation.Id);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // MongoDB doesn't require explicit save changes like EF Core
        // Changes are persisted immediately with each operation
        logger.LogDebug("SaveChangesAsync called - MongoDB persists changes immediately");
        return Task.CompletedTask;
    }

    public async Task<Conversation> AddRoundAsync(string id, ConversationRound conversationRound, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Adding round to conversation with ID: {Id}", id);

        var filter = Builders<Conversation>.Filter.Eq(c => c.Id, id);
        var update = Builders<Conversation>.Update.Push(c => c.Rounds, conversationRound);

        var result = await conversationCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
        {
            logger.LogWarning("No conversation found with ID: {Id} for adding round", id);
            throw new InvalidOperationException($"Conversation with ID {id} not found for adding round");
        }

        logger.LogDebug("Successfully added round to conversation with ID: {Id}", id);
        return await GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Conversation with ID {id} not found after adding round");
    }

    public async Task<Conversation> UpdateRoundAsync(string id, ChatResponse chatResponse, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Updating last round with chat response for conversation ID: {Id}", id);

        // First, get the conversation to find the last round index
        var conversation = await GetByIdAsync(id, cancellationToken);
        if (conversation == null)
        {
            logger.LogWarning("No conversation found with ID: {Id} for updating round", id);
            throw new InvalidOperationException($"Conversation with ID {id} not found for updating round");
        }

        if (conversation.Rounds.Count == 0)
        {
            logger.LogWarning("No rounds found in conversation with ID: {Id} for updating", id);
            throw new InvalidOperationException($"No rounds found in conversation with ID {id} for updating");
        }

        var lastRoundIndex = conversation.Rounds.Count - 1;

        // Update the response of the last round
        var filter = Builders<Conversation>.Filter.Eq(c => c.Id, id);
        var update = Builders<Conversation>.Update.Set($"Rounds.{lastRoundIndex}.Response", chatResponse);

        var result = await conversationCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
        {
            logger.LogWarning("Failed to update round in conversation with ID: {Id}", id);
            throw new InvalidOperationException($"Failed to update round in conversation with ID {id}");
        }

        logger.LogDebug("Successfully updated round in conversation with ID: {Id}", id);
        return await GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Conversation with ID {id} not found after adding round");
    }
}
