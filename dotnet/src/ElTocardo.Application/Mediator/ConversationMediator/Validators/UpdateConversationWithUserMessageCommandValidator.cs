using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using FluentValidation;

namespace ElTocardo.Application.Mediator.ConversationMediator.Validators;

public class UpdateConversationWithUserMessageCommandValidator : AbstractValidator<UpdateConversationAddNewRoundCommand>
{
    public UpdateConversationWithUserMessageCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Key (conversation title) is required")
            .MaximumLength(200)
            .WithMessage("Key cannot exceed 200 characters");

        RuleFor(x => x.UserMessage)
            .NotNull()
            .WithMessage("UserMessage is required");
    }
}
