using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Warehouses.Commands.CreateWarehouse;

public class CreateWarehouseCommandHandler
    : IRequestHandler<CreateWarehouseCommand, Guid>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWarehouseCommandHandler(
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork)
    {
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateWarehouseCommand request,
        CancellationToken cancellationToken)
    {
        var warehouse = new Warehouse(
            request.Code,
            request.Name,
            request.ArabicName,
            request.Manager,
            request.Phone,
            request.Address,
            request.Notes);

        _warehouseRepository.Add(warehouse);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return warehouse.Id;
    }
}