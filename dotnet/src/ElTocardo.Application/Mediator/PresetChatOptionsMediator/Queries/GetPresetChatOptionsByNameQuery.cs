using ElTocardo.Application.Mediator.Common.Queries;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Queries;

public record GetPresetChatOptionsByNameQuery(string Key) : GetByKeyQueryBase<string>(Key);
