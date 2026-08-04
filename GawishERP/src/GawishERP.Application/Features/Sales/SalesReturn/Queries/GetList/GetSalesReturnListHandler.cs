using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Queries.GetList;

public sealed class GetSalesReturnListHandler
    : IRequestHandler<GetSalesReturnListQuery, GetSalesReturnListResponse>
{
    private readonly ISalesReturnRepository _salesReturnRepository;

    public GetSalesReturnListHandler(
        ISalesReturnRepository salesReturnRepository)
    {
        _salesReturnRepository = salesReturnRepository;
    }

    public async Task<GetSalesReturnListResponse> Handle(
        GetSalesReturnListQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) =
            await _salesReturnRepository.GetAllAsync(
                request.Search,
                request.CustomerId,
                request.WarehouseId,
                request.FromDate,
                request.ToDate,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        return new GetSalesReturnListResponse
        {
            Items = items
                .Select(x => new SalesReturnListItemDto
                {
                    Id = x.Id,

                    DocumentNumber = x.DocumentNumber,

                    DocumentDate = x.DocumentDate,

                    Status = x.Status.ToString(),

                    CustomerId = x.CustomerId,

                    CustomerName = x.Customer.Name,

                    WarehouseId = x.WarehouseId,

                    WarehouseName = x.Warehouse.Name,

                    ReturnReason = x.ReturnReason,

                    TotalAmount = x.TotalAmount
                })
                .ToList(),

            TotalCount = totalCount,

            PageNumber = request.PageNumber,

            PageSize = request.PageSize
        };
    }
}