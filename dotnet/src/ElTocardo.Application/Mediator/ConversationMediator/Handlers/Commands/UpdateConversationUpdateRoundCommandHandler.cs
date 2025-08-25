using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Application.Mediator.ConversationMediator.Mappers;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.ConversationMediator.Handlers.Commands;

public class UpdateConversationUpdateRoundCommandHandler(
    IConversationRepository repository,
    ILogger<UpdateConversationUpdateRoundCommandHandler> logger,
    IValidator<UpdateConversationUpdateRoundCommand> validator)
    : CommandHandlerBase<UpdateConversationUpdateRoundCommand, Conversation>(logger)
{

    protected override async Task<Conversation> HandleAsyncImplementation(
        UpdateConversationUpdateRoundCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating Conversation: {ServerName}", command);

        // Validate command
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        // Save changes
        var conversation = await repository.UpdateRoundAsync(command.Id, command.ChatResponse, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Conversation updated successfully: {ServerName}", command);

        return conversation;
    }
}
