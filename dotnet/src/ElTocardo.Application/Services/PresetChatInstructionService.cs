using ElTocardo.Application.Dtos.PresetChatInstruction;
using ElTocardo.Domain.Models;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Commands;
using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Queries;

namespace ElTocardo.Application.Services;

public class PresetChatInstructionEndpointService(
    ICommandHandler<CreatePresetChatInstructionCommand, Guid> createHandler,
    ICommandHandler<UpdatePresetChatInstructionCommand> updateHandler,
    ICommandHandler<DeletePresetChatInstructionCommand> deleteHandler,
    IQueryHandler<GetAllPresetChatInstructionsQuery, List<PresetChatInstructionDto>> getAllHandler,
    IQueryHandler<GetPresetChatInstructionByNameQuery, PresetChatInstructionDto> getByNameHandler)
    : IPresetChatInstructionEndpointService
{
    public async Task<Result<Guid>> CreateAsync(string name, string description, string contentType, string content, CancellationToken cancellationToken = default)
    {
        var command = new CreatePresetChatInstructionCommand(name, description, contentType, content);
        return await createHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<VoidResult> UpdateAsync(string name, string description, string contentType, string content, CancellationToken cancellationToken = default)
    {
        var command = new UpdatePresetChatInstructionCommand(name, description, contentType, content);
        return await updateHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<VoidResult> DeleteAsync(string name, CancellationToken cancellationToken = default)
    {
        var command = new DeletePresetChatInstructionCommand(name);
        return await deleteHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<Result<IList<PresetChatInstructionDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
    var result = await getAllHandler.HandleAsync(new GetAllPresetChatInstructionsQuery(), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ReadError();
        }

        return result.ReadValue();
    }

    public async Task<Result<PresetChatInstructionDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var result = await getByNameHandler.HandleAsync(new GetPresetChatInstructionByNameQuery(name), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ReadError();
        }

        return result.ReadValue();
    }
}
