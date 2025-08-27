using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using ElTocardo.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.ConversationMediator.Handlers.Commands;

public class UpdateConversationAddNewRoundCommandHandler(
    IConversationRepository repository,
    ILogger<UpdateConversationAddNewRoundCommandHandler> logger,
    IValidator<UpdateConversationAddNewRoundCommand> validator)

    : ICommandHandler<UpdateConversationAddNewRoundCommand, Conversation>
{

    public async Task<Result<Conversation>> HandleAsync(
        UpdateConversationAddNewRoundCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating Conversation: {ServerName}", command);

        // Validate command
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationException(validationResult.ToString());
        }
        var conversationRound = new ConversationRound(command.UserMessage, command.Options, command.Provider ?? string.Empty);
        // Save changes
        return await repository.AddRoundAsync(command.Id,conversationRound, cancellationToken);
    }
}
