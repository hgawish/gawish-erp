using GawishERP.Application.Common.Exceptions;
using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Warehouses.Commands.UpdateWarehouse;

public class UpdateWarehouseCommandHandler
    : IRequestHandler<UpdateWarehouseCommand, Guid>
{
    private readonly IWarehouseRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWarehouseCommandHandler(
        IWarehouseRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        UpdateWarehouseCommand request,
        CancellationToken cancellationToken)
    {
        var warehouse = await _repository.GetByIdAsync(request.Id);

        if (warehouse is null)
        {
            throw new NotFoundException(
                $"Warehouse '{request.Id}' was not found.");
        }

        warehouse.Update(
            request.Name,
            request.ArabicName,
            request.Manager,
            request.Phone,
            request.Address,
            request.Notes);

        _repository.Update(warehouse);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return warehouse.Id;
    }
}