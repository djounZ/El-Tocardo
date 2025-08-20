namespace ElTocardo.Application.Mediator.Common.Commands;

public abstract record UpdateCommandBase<TKey>(TKey Key);
public abstract record DeleteCommandBase<TKey>(TKey Key);
