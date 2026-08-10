using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Create;

public sealed record CreateSalesCommand
    : IRequest<CreateSalesResponse>
{
    public DateTime DocumentDate { get; init; }

    public Guid FiscalYearId { get; init; }

    public Guid? CompanyId { get; init; }

    public Guid? BranchId { get; init; }

    public Guid CustomerId { get; init; }

    public Guid WarehouseId { get; init; }

    public string Currency { get; init; } = "EGP";

    public decimal ExchangeRate { get; init; } = 1;

    public string? Notes { get; init; }

    public List<CreateSalesLineDto> Lines { get; init; } = new();
}