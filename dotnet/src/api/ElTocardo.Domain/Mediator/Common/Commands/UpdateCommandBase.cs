namespace ElTocardo.Domain.Mediator.Common.Commands;

public abstract record UpdateCommandBase<TKey>(TKey Id);
