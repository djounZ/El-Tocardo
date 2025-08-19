using ElTocardo.Application.Commands.PresetChatOptions;
using FluentValidation;

namespace ElTocardo.Application.Validators.PresetChatOptions;

public class UpdatePresetChatOptionsCommandValidator : AbstractValidator<UpdatePresetChatOptionsCommand>
{
    public UpdatePresetChatOptionsCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Preset name is required")
            .MaximumLength(255).WithMessage("Preset name must not exceed 255 characters");

        RuleFor(x => x.Instructions)
            .MinimumLength(200).When(x => !string.IsNullOrEmpty(x.Instructions))
            .WithMessage("Instructions must be at least 200 characters if set");

        RuleFor(x => x.Temperature)
            .InclusiveBetween(0, 2).When(x => x.Temperature.HasValue)
            .WithMessage("Temperature must be between 0 and 2");

        RuleFor(x => x.MaxOutputTokens)
            .GreaterThan(0).When(x => x.MaxOutputTokens.HasValue)
            .WithMessage("MaxOutputTokens must be positive");

        RuleFor(x => x.TopP)
            .InclusiveBetween(0, 1).When(x => x.TopP.HasValue)
            .WithMessage("TopP must be between 0 and 1");

        RuleFor(x => x.TopK)
            .GreaterThanOrEqualTo(0).When(x => x.TopK.HasValue)
            .WithMessage("TopK must be non-negative");

        RuleFor(x => x.FrequencyPenalty)
            .InclusiveBetween(-2, 2).When(x => x.FrequencyPenalty.HasValue)
            .WithMessage("FrequencyPenalty must be between -2 and 2");

        RuleFor(x => x.PresencePenalty)
            .InclusiveBetween(-2, 2).When(x => x.PresencePenalty.HasValue)
            .WithMessage("PresencePenalty must be between -2 and 2");

        RuleFor(x => x.ModelId)
            .MaximumLength(255).When(x => x.ModelId != null)
            .WithMessage("ModelId must not exceed 255 characters");

        RuleFor(x => x.StopSequences)
            .Must(list => list == null || list.All(s => s.Length <= 100))
            .WithMessage("Each stop sequence must not exceed 100 characters");

        When(x => x.ToolMode != null && x.ToolMode.ToLowerInvariant() == "required", () =>
        {
            RuleFor(x => x.RequiredFunctionName)
                .NotEmpty().WithMessage("RequiredFunctionName is required when ToolMode is 'required'")
                .MaximumLength(255).WithMessage("RequiredFunctionName must not exceed 255 characters");
        });
    }
}