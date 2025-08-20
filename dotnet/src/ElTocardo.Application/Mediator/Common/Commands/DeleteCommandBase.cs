namespace ElTocardo.Application.Mediator.Common.Commands;

public abstract record DeleteCommandBase<TKey>(TKey Key);
