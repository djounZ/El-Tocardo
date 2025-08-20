using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using FluentValidation;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Validators;

public class UpdatePresetChatOptionsCommandValidator : AbstractValidator<UpdatePresetChatOptionsCommand>
{
    public UpdatePresetChatOptionsCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(255)
            .WithMessage("Name cannot exceed 255 characters");

        RuleFor(x => x.Temperature)
            .GreaterThanOrEqualTo(0f)
            .LessThanOrEqualTo(2f)
            .When(x => x.Temperature.HasValue)
            .WithMessage("Temperature must be between 0 and 2");

        RuleFor(x => x.MaxOutputTokens)
            .GreaterThan(0)
            .When(x => x.MaxOutputTokens.HasValue)
            .WithMessage("MaxOutputTokens must be greater than 0");

        RuleFor(x => x.TopP)
            .GreaterThanOrEqualTo(0f)
            .LessThanOrEqualTo(1f)
            .When(x => x.TopP.HasValue)
            .WithMessage("TopP must be between 0 and 1");

        RuleFor(x => x.TopK)
            .GreaterThan(0)
            .When(x => x.TopK.HasValue)
            .WithMessage("TopK must be greater than 0");

        RuleFor(x => x.FrequencyPenalty)
            .GreaterThanOrEqualTo(-2f)
            .LessThanOrEqualTo(2f)
            .When(x => x.FrequencyPenalty.HasValue)
            .WithMessage("FrequencyPenalty must be between -2 and 2");

        RuleFor(x => x.PresencePenalty)
            .GreaterThanOrEqualTo(-2f)
            .LessThanOrEqualTo(2f)
            .When(x => x.PresencePenalty.HasValue)
            .WithMessage("PresencePenalty must be between -2 and 2");
    }
}
