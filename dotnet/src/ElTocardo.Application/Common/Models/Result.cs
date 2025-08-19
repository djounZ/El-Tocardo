namespace ElTocardo.Application.Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public Exception? Exception { get; private set; }

    private Result(bool isSuccess, T? value, string? error, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Exception = exception;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, default, error);
    }

    public static Result<T> Failure(string error, Exception exception)
    {
        return new Result<T>(false, default, error, exception);
    }
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public Exception? Exception { get; private set; }

    private Result(bool isSuccess, string? error, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Exception = exception;
    }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(string error)
    {
        return new Result(false, error);
    }

    public static Result Failure(string error, Exception exception)
    {
        return new Result(false, error, exception);
    }
}
