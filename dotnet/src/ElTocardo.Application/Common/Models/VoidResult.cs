namespace ElTocardo.Application.Common.Models;

public class VoidResult
{
    protected readonly Exception? Exception;

    protected VoidResult(Exception? exception)
    {
        Exception = exception;
    }

    public static VoidResult Success { get; }= new(null);

    public bool IsSuccess => Exception == null;

    public Exception ReadError()
    {
        return Exception ?? throw new InvalidOperationException("No exception available, the result must be valid.");
    }

    public static implicit operator VoidResult(Exception exception)
    {
        return new VoidResult(exception);
    }



    public static implicit operator VoidResult(string errorMessage)
    {
        return new ArgumentException(errorMessage);
    }
}
