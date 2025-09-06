using ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;
using FluentValidation;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Validators;

public class UpdateUserExternalTokenCommandValidator : AbstractValidator<UpdateUserExternalTokenCommand>
{
    public UpdateUserExternalTokenCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotNull().WithMessage("Key is required");
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required");
    }
}
