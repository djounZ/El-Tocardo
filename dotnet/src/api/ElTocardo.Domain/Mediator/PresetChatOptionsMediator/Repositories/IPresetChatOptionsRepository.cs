using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

namespace ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;

public interface IPresetChatOptionsRepository : IEntityRepository<PresetChatOptions, Guid, string>;
