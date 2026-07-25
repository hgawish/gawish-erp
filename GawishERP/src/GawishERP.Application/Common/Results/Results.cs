using System.Diagnostics.CodeAnalysis;

namespace GawishERP.Application.Common.Results;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Successful result cannot contain an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failed result must contain an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success()
    {
        return new Result(true, Error.None);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value, true, Error.None);
    }

    public static Result<T> Failure<T>(Error error)
    {
        return new Result<T>(default, false, error);
    }
}

public sealed class Result<T> : Result
{
    internal Result(
        T? value,
        bool isSuccess,
        Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    [NotNull]
    public T? Value { get; }

    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }
}