using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using FluentValidation;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Validators;

public class DeletePresetChatOptionsCommandValidator : AbstractValidator<DeletePresetChatOptionsCommand>
{
    public DeletePresetChatOptionsCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}
