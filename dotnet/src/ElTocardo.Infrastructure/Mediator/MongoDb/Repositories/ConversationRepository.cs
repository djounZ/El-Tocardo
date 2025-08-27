using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ElTocardo.Infrastructure.Mediator.MongoDb.Repositories;

public class ConversationRepository(ILogger<ConversationRepository> logger, IMongoDatabase mongoDatabase) : IConversationRepository
{
    private readonly IMongoCollection<Conversation> _conversationCollection = mongoDatabase.GetCollection<Conversation>(nameof(Conversation));

    public async Task<Result<IEnumerable<Conversation>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting all conversations");
        try
        {

            var conversations = await _conversationCollection.Find(_ => true).ToListAsync(cancellationToken);
            logger.LogDebug("Retrieved {Count} conversations", conversations.Count);
            return conversations;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting conversations");
            return ex;
        }
    }

    public async Task<Result<Conversation>> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Getting conversation by key: {Key}", key);
            var conversation = await _conversationCollection.Find(c => c.Title == key).FirstOrDefaultAsync(cancellationToken);
            if (conversation == null)
            {
                return new KeyNotFoundException($"Key {key} not found");
            }
            return conversation;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting conversation by key");
            return ex;
        }
    }

    public async Task<VoidResult> AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        try
        {

            logger.LogDebug("Adding conversation with ID: {Id}", conversation.Id);
            await _conversationCollection.InsertOneAsync(conversation, cancellationToken: cancellationToken);
            logger.LogDebug("Successfully added conversation with ID: {Id}", conversation.Id);
            return VoidResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding conversation");
            return ex;
        }
    }

    public async Task<VoidResult> UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Updating conversation with ID: {Id}", conversation.Id);
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, conversation.Id);
            var result = await _conversationCollection.ReplaceOneAsync(filter, conversation, cancellationToken: cancellationToken);

            if (result.MatchedCount == 0)
            {
                logger.LogWarning("No conversation found with ID: {Id} for update", conversation.Id);
                return new InvalidOperationException($"Conversation with ID {conversation.Id} not found for update");
            }

            logger.LogDebug("Successfully updated conversation with ID: {Id}", conversation.Id);
            return VoidResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating conversation");
            return ex;
        }
    }

    public async Task<VoidResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {

            logger.LogDebug("Deleting conversation with ID: {Id}", id);
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, id);
            var result = await _conversationCollection.DeleteOneAsync(filter, cancellationToken);

            if (result.DeletedCount == 0)
            {
                logger.LogWarning("No conversation found with ID: {Id} for deletion", id);
                return new InvalidOperationException($"Conversation with ID {id} not found for deletion");
            }

            logger.LogDebug("Successfully deleted conversation with ID: {Id}", id);
            return VoidResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting conversation");
            return ex;
        }
    }

    public async Task<Result<Conversation>> AddRoundAsync(string id, ConversationRound conversationRound, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Adding round to conversation with ID: {Id}", id);

            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, id);

            // Build the update operations
            var updates = new List<UpdateDefinition<Conversation>>
            {
                Builders<Conversation>.Update.Push(c => c.Rounds, conversationRound)
            };

            // Update current provider if the round has a provider
            if (!string.IsNullOrEmpty(conversationRound.Provider))
            {
                updates.Add(Builders<Conversation>.Update.Set(c => c.CurrentProvider, conversationRound.Provider));
                logger.LogDebug("Updating current provider to: {Provider}", conversationRound.Provider);
            }

            // Update current options if the round has options
            if (conversationRound.Options != null)
            {
                updates.Add(Builders<Conversation>.Update.Set(c => c.CurrentOptions, conversationRound.Options));
                logger.LogDebug("Updating current options");
            }
            updates.Add(Builders<Conversation>.Update.Set(c => c.UpdatedAt, DateTimeOffset.UtcNow));
            var combinedUpdate = Builders<Conversation>.Update.Combine(updates);
            var options = new FindOneAndUpdateOptions<Conversation>
            {
                ReturnDocument = ReturnDocument.After
            };

            var updatedConversation = await _conversationCollection.FindOneAndUpdateAsync(filter, combinedUpdate, options, cancellationToken);

            if (updatedConversation == null)
            {
                logger.LogWarning("No conversation found with ID: {Id} for adding round", id);
                return new InvalidOperationException($"Conversation with ID {id} not found for adding round");
            }

            logger.LogDebug("Successfully added round to conversation with ID: {Id}", id);
            return updatedConversation;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding round to conversation");
            return ex;
        }
    }

    public async Task<Result<Conversation>> UpdateRoundAsync(string id, ChatResponse chatResponse, CancellationToken cancellationToken = default)
    {
        try
        {

            logger.LogDebug("Updating last round with chat response for conversation ID: {Id}", id);

            // First, get the conversation to find the last round index
            var byKeyAsync = await GetByKeyAsync(id, cancellationToken);
            if (!byKeyAsync.IsSuccess)
            {
                logger.LogWarning("No conversation found with ID: {Id} for updating round", id);
                throw new InvalidOperationException($"Conversation with ID {id} not found for updating round");
            }

            var conversation = byKeyAsync.ReadValue();
            if (conversation.Rounds.Count == 0)
            {
                logger.LogWarning("No rounds found in conversation with ID: {Id} for updating", id);
                throw new InvalidOperationException($"No rounds found in conversation with ID {id} for updating");
            }

            var lastRoundIndex = conversation.Rounds.Count - 1;

            // Update the response of the last round using FindOneAndUpdate
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, id);
            var dateTimeOffset = DateTimeOffset.UtcNow;
            var updates = new List<UpdateDefinition<Conversation>>
            {
                Builders<Conversation>.Update.Set($"Rounds.{lastRoundIndex}.Response", chatResponse),
                Builders<Conversation>.Update.Set($"Rounds.{lastRoundIndex}.UpdatedAt", dateTimeOffset),
                Builders<Conversation>.Update.Set(c => c.UpdatedAt, dateTimeOffset)
            };
            var combinedUpdate = Builders<Conversation>.Update.Combine(updates);
            var options = new FindOneAndUpdateOptions<Conversation>
            {
                ReturnDocument = ReturnDocument.After
            };

            var updatedConversation = await _conversationCollection.FindOneAndUpdateAsync(filter, combinedUpdate, options, cancellationToken);

            if (updatedConversation == null)
            {
                logger.LogWarning("Failed to update round in conversation with ID: {Id}", id);
                return new InvalidOperationException($"Failed to update round in conversation with ID {id}");
            }

            logger.LogDebug("Successfully updated round in conversation with ID: {Id}", id);
            return updatedConversation;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating conversation");
            return ex;
        }
    }
}
