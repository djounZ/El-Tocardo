using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.ConversationMediator.Handlers.Commands;

public class UpdateConversationAddNewRoundCommandHandler(
    IConversationRepository repository,
    ILogger<UpdateConversationAddNewRoundCommandHandler> logger,
    IValidator<UpdateConversationAddNewRoundCommand> validator)

    : CommandHandlerBase<UpdateConversationAddNewRoundCommand, Conversation>(logger)
{

    protected override async Task<Conversation> HandleAsyncImplementation(
        UpdateConversationAddNewRoundCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating Conversation: {ServerName}", command);

        // Validate command
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var conversationRound = new ConversationRound(command.UserMessage, command.Options, command.Provider ?? string.Empty);
        // Save changes
        var conversation = await repository.AddRoundAsync(command.Id,conversationRound, cancellationToken);

        logger.LogInformation("Conversation updated successfully: {ServerName}", command);

        return conversation;
    }
}
