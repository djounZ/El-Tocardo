namespace ElTocardo.Domain.Mediator.Commands;

public abstract record UpdateCommandBase<TKey>(TKey Id);
