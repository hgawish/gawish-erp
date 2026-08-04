namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Cancel;

public sealed class CancelSalesReturnResponse
{
    public Guid Id { get; set; }

    public string DocumentNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}