using MediatR;

namespace GawishERP.Application.Features.Warehouses.Commands.ActivateWarehouse;

public record ActivateWarehouseCommand(Guid Id) : IRequest;