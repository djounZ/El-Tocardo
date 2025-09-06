namespace ElTocardo.Domain.Mediator.Common.Commands;

public abstract record DeleteCommandBase<TKey>(TKey Key);
