using MediatR;

namespace GawishERP.Application.Features.Warehouses.Commands.DeactivateWarehouse;

public record DeactivateWarehouseCommand(Guid Id) : IRequest;