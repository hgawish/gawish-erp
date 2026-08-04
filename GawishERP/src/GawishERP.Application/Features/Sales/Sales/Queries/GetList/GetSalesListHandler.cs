using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Queries.GetList;

public sealed class GetSalesListHandler
    : IRequestHandler<GetSalesListQuery, GetSalesListResponse>
{
    private readonly ISalesRepository _salesRepository;

    public GetSalesListHandler(
        ISalesRepository salesRepository)
    {
        _salesRepository = salesRepository;
    }

    public Task<GetSalesListResponse> Handle(
        GetSalesListQuery request,
        CancellationToken cancellationToken)
    {
        var query = _salesRepository.GetQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.DocumentNumber.Contains(request.Search) ||
                x.Customer.Name.Contains(request.Search));
        }

        if (request.CustomerId.HasValue)
        {
            query = query.Where(x =>
                x.CustomerId == request.CustomerId.Value);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(x =>
                x.WarehouseId == request.WarehouseId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x =>
                x.DocumentDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x =>
                x.DocumentDate <= request.ToDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<DocumentStatus>(
                request.Status,
                true,
                out var status))
        {
            query = query.Where(x =>
                x.Status == status);
        }

        var totalCount = query.Count();

        var items = query
            .OrderByDescending(x => x.DocumentDate)
            .ThenByDescending(x => x.DocumentNumber)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SalesListItemDto
            {
                Id = x.Id,
                DocumentNumber = x.DocumentNumber,
                DocumentDate = x.DocumentDate,
                CustomerName = x.Customer.Name,
                WarehouseName = x.Warehouse.Name,
                NetTotal = x.NetTotal,
                Currency = x.Currency,
                Status = x.Status.ToString()
            })
            .ToList();

        return Task.FromResult(new GetSalesListResponse
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}