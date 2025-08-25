namespace ElTocardo.Application.Mediator.Common.Commands;

public abstract record UpdateCommandBase<TKey>(TKey Id);
