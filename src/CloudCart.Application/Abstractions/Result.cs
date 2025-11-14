namespace CloudCart.Application.Abstractions;

/// <summary>
/// Result pattern for operations without return data
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Successful result cannot have an error");

        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Failed result must have an error");

        IsSuccess = isSuccess;
        Error = error ?? string.Empty;
    }

    public static Result Success() => new(true, string.Empty);

    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);

    public static Result<T> Failure<T>(string error) => new(default!, false, error);
}

/// <summary>
/// Result pattern for operations with return data
/// </summary>
public class Result<T> : Result
{
    public T Value { get; }

    internal Result(T value, bool isSuccess, string error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static implicit operator Result<T>(T value) => Success(value);
}
