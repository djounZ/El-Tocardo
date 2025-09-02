namespace ElTocardo.Domain.Mediator.Commands;

public abstract record DeleteCommandBase<TKey>(TKey Key);
