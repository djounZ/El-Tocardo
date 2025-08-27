namespace ElTocardo.Domain.Models;

public class Result<T> : VoidResult
{

    private readonly T? _value;
    protected Result(Exception exception) : base(exception)
    {
    }

    protected Result(T value) : base(null)
    {
        _value = value;
    }


    public T ReadValue()
    {
        if (_value != null && IsSuccess)
        {
            return _value;
        }

        throw new InvalidOperationException("Cannot ReadValue On Invalid Result", Exception);
    }

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Exception exception)
    {
        return new Result<T>(exception);
    }

}
