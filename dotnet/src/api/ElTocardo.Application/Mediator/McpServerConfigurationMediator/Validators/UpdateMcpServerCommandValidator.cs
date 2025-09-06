using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.ValueObjects;
using FluentValidation;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Validators;

public class UpdateMcpServerCommandValidator : AbstractValidator<UpdateMcpServerCommand>
{
    public UpdateMcpServerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Server name is required")
            .MaximumLength(255)
            .WithMessage("Server name must not exceed 255 characters");

        RuleFor(x => x.Category)
            .MaximumLength(100)
            .WithMessage("Category must not exceed 100 characters");

        When(x => x.TransportType == McpServerTransportType.Stdio, () =>
        {
            RuleFor(x => x.Command)
                .NotEmpty()
                .WithMessage("Command is required for Stdio transport type")
                .MaximumLength(500)
                .WithMessage("Command must not exceed 500 characters");
        });

        When(x => x.TransportType == McpServerTransportType.Http, () =>
        {
            RuleFor(x => x.Endpoint)
                .NotNull()
                .WithMessage("Endpoint is required for Http transport type");
        });

        RuleFor(x => x.TransportType)
            .IsInEnum()
            .WithMessage("Invalid transport type");
    }
}
