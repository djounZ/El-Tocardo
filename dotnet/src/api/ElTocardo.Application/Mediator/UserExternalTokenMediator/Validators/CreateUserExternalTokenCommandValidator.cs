using ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;
using FluentValidation;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Validators;

public class CreateUserExternalTokenCommandValidator : AbstractValidator<CreateUserExternalTokenCommand>
{
    public CreateUserExternalTokenCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Provider is required");
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required");
    }
}
