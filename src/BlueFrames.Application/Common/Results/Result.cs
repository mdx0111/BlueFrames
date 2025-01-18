namespace BlueFrames.Application.Common.Results;

public class Result
{
    public bool IsSuccess { get; set; }
    public string Error { get; set; }

    public bool IsFailure => !IsSuccess;

    protected Result(bool success, string error)
    {
        IsSuccess = success;
        Error = error;
    }

    public static Result Failure(string message)
    {
        return new Result(false, message);
    }

    public static Result<T> Failure<T>(string message)
    {
        return new Result<T>(default, false, message);
    }

    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
            {
                return result;
            }
        }

        return Success();
    }
}

public class Result<T> : Result
{
    public T Value { get; set; }

    protected internal Result(T value, bool success, string error)
        : base(success, error)
    {
        Value = value;
    }
}
