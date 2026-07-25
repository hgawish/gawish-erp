namespace GawishERP.Application.Common.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}