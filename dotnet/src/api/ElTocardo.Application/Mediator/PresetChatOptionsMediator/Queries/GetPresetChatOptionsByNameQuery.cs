using ElTocardo.Domain.Mediator.Queries;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Queries;

public record GetPresetChatOptionsByNameQuery(string Key) : GetByKeyQueryBase<string>(Key);
