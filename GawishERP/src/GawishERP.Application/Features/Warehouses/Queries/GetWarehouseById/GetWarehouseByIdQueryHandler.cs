using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Warehouses.Queries.GetWarehouseById;

public class GetWarehouseByIdQueryHandler
    : IRequestHandler<GetWarehouseByIdQuery, GetWarehouseByIdResponse?>
{
    private readonly IWarehouseRepository _repository;

    public GetWarehouseByIdQueryHandler(
        IWarehouseRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetWarehouseByIdResponse?> Handle(
        GetWarehouseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var warehouse = await _repository.GetByIdAsync(request.Id);

        if (warehouse is null)
            return null;

        return new GetWarehouseByIdResponse
        {
            Id = warehouse.Id,
            Code = warehouse.Code,
            Name = warehouse.Name,
            ArabicName = warehouse.ArabicName,
            Manager = warehouse.Manager,
            Phone = warehouse.Phone,
            Address = warehouse.Address,
            Notes = warehouse.Notes,
            IsActive = warehouse.IsActive
        };
    }
}