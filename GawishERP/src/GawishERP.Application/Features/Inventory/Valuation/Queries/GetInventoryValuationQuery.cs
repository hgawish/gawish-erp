using MediatR;

namespace GawishERP.Application.Features.Inventory.Valuation.Queries;

public sealed record GetInventoryValuationQuery(
    Guid? ProductId = null,
    Guid? WarehouseId = null)
    : IRequest<GetInventoryValuationResponse>;