using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Create;

public sealed record CreatePurchaseReturnCommand
    : IRequest<CreatePurchaseReturnResponse>
{
    //=========================================================
    // Organization
    //=========================================================

    public Guid FiscalYearId { get; init; }

    public Guid? CompanyId { get; init; }

    public Guid? BranchId { get; init; }

    //=========================================================
    // Document
    //=========================================================

    /// <summary>
    /// ERP Document Date
    /// </summary>
    public DateTime DocumentDate { get; init; }

    /// <summary>
    /// Original Purchase Document
    /// </summary>
    public Guid PurchaseId { get; init; }

    public Guid SupplierId { get; init; }

    public Guid WarehouseId { get; init; }

    public string ReturnReason { get; init; } = string.Empty;

    public string? Notes { get; init; }

    //=========================================================
    // Lines
    //=========================================================

    public List<CreatePurchaseReturnLineDto> Lines { get; init; } = [];
}

public sealed record CreatePurchaseReturnLineDto
{
    /// <summary>
    /// Original Purchase Line
    /// </summary>
    public Guid PurchaseLineId { get; init; }

    public Guid ProductId { get; init; }

    public decimal Quantity { get; init; }

    public decimal UnitCost { get; init; }

    public string? Notes { get; init; }
}