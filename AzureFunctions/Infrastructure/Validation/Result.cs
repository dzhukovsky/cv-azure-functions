namespace AzureFunctions.Infrastructure.Validation;

public readonly struct Result
{
    public readonly bool IsSucceed => string.IsNullOrEmpty(Failure);
    public readonly bool IsFailure => !IsSucceed;
    public readonly string? Failure { get; }

    public Result()
    {
    }

    public Result(string failure)
    {
        Failure = failure;
    }

    public static Result Success() => new();
    public static Result Error(string failure) => new(failure);

    public static Result<T> Success<T>(T value) => new(value);
    public static Result<T> Error<T>(string failure) => new(failure);
}

public readonly struct Result<T>
{
    private readonly T? _value;

    public readonly bool IsSucceed => string.IsNullOrEmpty(Failure);
    public readonly bool IsFailure => !IsSucceed;
    public readonly string Failure { get; }
    public T Value => IsSucceed
      ? _value!
      : throw new InvalidOperationException("There is no value");

    public Result()
    {
        Failure = "There is no value";
    }

    public Result(T value)
    {
        _value = value;
        Failure = string.Empty;
    }

    public Result(string failure)
    {
        Failure = failure;
    }

    public static implicit operator Result(Result<T> result)
        => result.IsSucceed ? Result.Success() : Result.Error(result.Failure!);
}
