using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Domain.Mediator.Common.Interfaces;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using ElTocardo.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.ConversationMediator.Handlers.Commands;

public class UpdateConversationUpdateRoundCommandHandler(
    IConversationRepository repository,
    ILogger<UpdateConversationUpdateRoundCommandHandler> logger,
    IValidator<UpdateConversationUpdateRoundCommand> validator)
    : ICommandHandler<UpdateConversationUpdateRoundCommand, Conversation>
{

    public async Task<Result<Conversation>> HandleAsync(
        UpdateConversationUpdateRoundCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating Conversation: {ServerName}", command);

        // Validate command
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationException(string.Concat(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        // Save changes
        return await repository.UpdateRoundAsync(command.Id, command.ChatResponse, cancellationToken);
    }
}
