using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using FluentValidation;

namespace ElTocardo.Application.Mediator.ConversationMediator.Validators;

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.UserMessage)
            .NotNull()
            .WithMessage("UserMessage is required");

        RuleFor(x => x.Provider)
            .MaximumLength(100)
            .WithMessage("Provider cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Provider));
    }
}
