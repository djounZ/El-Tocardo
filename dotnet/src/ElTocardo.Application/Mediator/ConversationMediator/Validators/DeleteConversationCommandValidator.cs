using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using FluentValidation;

namespace ElTocardo.Application.Mediator.ConversationMediator.Validators;

public class DeleteConversationCommandValidator : AbstractValidator<DeleteConversationCommand>
{
    public DeleteConversationCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Key (conversation title) is required")
            .MaximumLength(200)
            .WithMessage("Key cannot exceed 200 characters");
    }
}
