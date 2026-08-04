using GawishERP.Application.Common.Exceptions;
using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Warehouses.Commands.ActivateWarehouse;

public class ActivateWarehouseCommandHandler
    : IRequestHandler<ActivateWarehouseCommand>
{
    private readonly IWarehouseRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateWarehouseCommandHandler(
        IWarehouseRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ActivateWarehouseCommand request,
        CancellationToken cancellationToken)
    {
        var warehouse = await _repository.GetByIdAsync(request.Id);

        if (warehouse is null)
            throw new NotFoundException($"Warehouse '{request.Id}' was not found.");

        _repository.Activate(warehouse);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}