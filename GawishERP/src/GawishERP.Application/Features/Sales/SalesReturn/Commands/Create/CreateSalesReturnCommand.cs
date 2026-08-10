using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Create;

public sealed record CreateSalesReturnCommand
    : IRequest<CreateSalesReturnResponse>
{
    /// <summary>
    /// ERP Document Date
    /// </summary>
    public DateTime DocumentDate { get; init; }

    /// <summary>
    /// Fiscal Year
    /// </summary>
    public Guid FiscalYearId { get; init; }

    /// <summary>
    /// Company (Optional)
    /// </summary>
    public Guid? CompanyId { get; init; }

    /// <summary>
    /// Branch (Optional)
    /// </summary>
    public Guid? BranchId { get; init; }

    /// <summary>
    /// Original Sales Document
    /// </summary>
    public Guid SalesId { get; init; }

    public Guid CustomerId { get; init; }

    public Guid WarehouseId { get; init; }

    public string ReturnReason { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public List<CreateSalesReturnLineDto> Lines { get; init; } = new();
}