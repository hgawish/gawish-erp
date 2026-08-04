using GawishERP.Application.Common.Results;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Warehouses.Queries.GetAllWarehouses;

public class GetAllWarehousesHandler
    : IRequestHandler<GetAllWarehousesQuery, PagedResult<WarehouseDto>>
{
    private readonly IWarehouseRepository _repository;

    public GetAllWarehousesHandler(
        IWarehouseRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<WarehouseDto>> Handle(
        GetAllWarehousesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync(
            request.Search,
            request.IsActive,
            request.SortBy,
            request.Descending,
            request.PageNumber,
            request.PageSize);

        var items = result.Items
            .Select(x => new WarehouseDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ArabicName = x.ArabicName,
                Manager = x.Manager,
                Phone = x.Phone,
                IsActive = x.IsActive
            })
            .ToList();

        return new PagedResult<WarehouseDto>(
            items,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }
}