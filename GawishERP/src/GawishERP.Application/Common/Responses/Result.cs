namespace GawishERP.Application.Common.Responses;

public class Result<T>
{
    public bool IsSuccess { get; }

    public string Message { get; }

    public T? Value { get; }

    public List<string> Errors { get; }

    private Result(
        bool isSuccess,
        T? value,
        string message,
        List<string>? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(
        T value,
        string message = "")
    {
        return new Result<T>(
            true,
            value,
            message,
            null);
    }

    public static Result<T> Failure(
        string message,
        List<string>? errors = null)
    {
        return new Result<T>(
            false,
            default,
            message,
            errors);
    }
}