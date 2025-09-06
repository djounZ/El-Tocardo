using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using FluentValidation;

namespace ElTocardo.Application.Mediator.ConversationMediator.Validators;

public class UpdateConversationAddNewRoundCommandValidator : AbstractValidator<UpdateConversationAddNewRoundCommand>
{
    public UpdateConversationAddNewRoundCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Conversation ID is required")
            .Must(BeAValidGuid)
            .WithMessage("Conversation ID must be a valid GUID format");

        RuleFor(x => x.UserMessage)
            .NotNull()
            .WithMessage("UserMessage is required");

        RuleFor(x => x.Provider)
            .MaximumLength(100)
            .WithMessage("Provider cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Provider));
    }

    private static bool BeAValidGuid(string id)
    {
        return Guid.TryParse(id, out _);
    }
}
