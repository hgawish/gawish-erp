using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Commands.CreateOpeningBalanceDocument;

public class CreateOpeningBalanceDocumentCommand
    : IRequest<Guid>
{
    public Guid WarehouseId { get; init; }

    public DateTime DocumentDate { get; init; }

    public string? Notes { get; init; }

    public List<CreateOpeningBalanceLineDto> Lines { get; init; }
        = new();
}

public class CreateOpeningBalanceLineDto
{
    public Guid ProductId { get; init; }

    public decimal Quantity { get; init; }

    public decimal UnitCost { get; init; }

    public string? Notes { get; init; }
}