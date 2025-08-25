using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using FluentValidation;

namespace ElTocardo.Application.Mediator.ConversationMediator.Validators;

public class UpdateConversationUpdateRoundCommandValidator : AbstractValidator<UpdateConversationUpdateRoundCommand>
{
    public UpdateConversationUpdateRoundCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Conversation ID is required")
            .Must(BeAValidGuid)
            .WithMessage("Conversation ID must be a valid GUID format");

        RuleFor(x => x.ChatResponse)
            .NotNull()
            .WithMessage("ChatResponse is required");
    }

    private static bool BeAValidGuid(string id)
    {
        return Guid.TryParse(id, out _);
    }
}
