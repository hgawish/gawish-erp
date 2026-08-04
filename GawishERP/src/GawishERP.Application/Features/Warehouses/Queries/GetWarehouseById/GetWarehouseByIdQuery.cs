using MediatR;

namespace GawishERP.Application.Features.Warehouses.Queries.GetWarehouseById;

public record GetWarehouseByIdQuery(Guid Id)
    : IRequest<GetWarehouseByIdResponse?>;