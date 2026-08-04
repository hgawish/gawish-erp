using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Create;

public sealed record CreateSalesReturnCommand(
    DateTime DocumentDate,
    Guid SalesId,
    Guid CustomerId,
    Guid WarehouseId,
    string ReturnReason,
    string? Notes,
    List<CreateSalesReturnLineDto> Lines)
    : IRequest<CreateSalesReturnResponse>;