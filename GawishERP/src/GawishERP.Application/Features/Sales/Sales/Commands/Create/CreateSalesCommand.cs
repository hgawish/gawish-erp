using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Create;

public sealed record CreateSalesCommand(
    DateTime DocumentDate,
    Guid CustomerId,
    Guid WarehouseId,
    string Currency,
    decimal ExchangeRate,
    string? Notes,
    List<CreateSalesLineDto> Lines)
    : IRequest<CreateSalesResponse>;