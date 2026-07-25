namespace GawishERP.Application.Common.Exceptions;

public class ForbiddenException : BaseException
{
    public ForbiddenException(string message)
        : base(message)
    {
    }
}