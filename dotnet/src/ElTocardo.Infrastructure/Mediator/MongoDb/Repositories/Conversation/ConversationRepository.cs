using System.Linq.Expressions;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ElTocardo.Infrastructure.Mediator.MongoDb.Repositories.Conversation;

using System;
using System.Collections.Generic;

public class ConversationRepository(ILogger<ConversationRepository> logger, IMongoDatabase mongoDatabase)
    : MongoCollectionRepository<Domain.Mediator.ConversationMediator.Entities.Conversation, string, string>(logger, mongoDatabase), IConversationRepository
{
    protected override Expression<Func<Domain.Mediator.ConversationMediator.Entities.Conversation, bool>> GetByKeySelector(string id)
    {
        return  x => x.Id == id;
    }

    protected override FilterDefinition<Domain.Mediator.ConversationMediator.Entities.Conversation> GetUpdateFilter(Domain.Mediator.ConversationMediator.Entities.Conversation entity)
    {
        return Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Filter.Eq(c => c.Id, entity.Id);
    }

    protected override FilterDefinition<Domain.Mediator.ConversationMediator.Entities.Conversation> GetDeleteFilter(string key)
    {
        return Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Filter.Eq(c => c.Id, key);
    }

    public async Task<Result<Domain.Mediator.ConversationMediator.Entities.Conversation>> AddRoundAsync(string id, ConversationRound conversationRound, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Adding round to conversation with ID: {Id}", id);

            var filter = Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Filter.Eq(c => c.Id, id);

            // Build the update operations
            var updates = new List<UpdateDefinition<Domain.Mediator.ConversationMediator.Entities.Conversation>>
            {
                Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Update.Push(c => c.Rounds, conversationRound)
            };

            // Update current provider if the round has a provider
            if (!string.IsNullOrEmpty(conversationRound.Provider))
            {
                updates.Add(Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Update.Set(c => c.CurrentProvider, conversationRound.Provider));
                logger.LogDebug("Updating current provider to: {Provider}", conversationRound.Provider);
            }

            // Update current options if the round has options
            if (conversationRound.Options != null)
            {
                updates.Add(Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Update.Set(c => c.CurrentOptions, conversationRound.Options));
                logger.LogDebug("Updating current options");
            }
            updates.Add(Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Update.Set(c => c.UpdatedAt, DateTimeOffset.UtcNow));
            var combinedUpdate = Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Update.Combine(updates);
            var options = new FindOneAndUpdateOptions<Domain.Mediator.ConversationMediator.Entities.Conversation>
            {
                ReturnDocument = ReturnDocument.After
            };

            var updatedConversation = await EntityCollection.FindOneAndUpdateAsync(filter, combinedUpdate, options, cancellationToken);

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

    public async Task<Result<Domain.Mediator.ConversationMediator.Entities.Conversation>> UpdateRoundAsync(string id, ChatResponse chatResponse, CancellationToken cancellationToken = default)
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
            var filter = Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Filter.Eq(c => c.Id, id);
            var dateTimeOffset = DateTimeOffset.UtcNow;
            var updates = new List<UpdateDefinition<Domain.Mediator.ConversationMediator.Entities.Conversation>>
            {
                Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Update.Set($"Rounds.{lastRoundIndex}.Response", chatResponse),
                Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Update.Set($"Rounds.{lastRoundIndex}.UpdatedAt", dateTimeOffset),
                Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Update.Set(c => c.UpdatedAt, dateTimeOffset)
            };
            var combinedUpdate = Builders<Domain.Mediator.ConversationMediator.Entities.Conversation>.Update.Combine(updates);
            var options = new FindOneAndUpdateOptions<Domain.Mediator.ConversationMediator.Entities.Conversation>
            {
                ReturnDocument = ReturnDocument.After
            };

            var updatedConversation = await EntityCollection.FindOneAndUpdateAsync(filter, combinedUpdate, options, cancellationToken);

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
