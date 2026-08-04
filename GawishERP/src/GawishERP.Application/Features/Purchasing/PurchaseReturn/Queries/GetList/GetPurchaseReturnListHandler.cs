using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Queries.GetList;

public sealed class GetPurchaseReturnListHandler
    : IRequestHandler<GetPurchaseReturnListQuery, List<PurchaseReturnListItemDto>>
{
    private readonly IPurchaseReturnRepository _purchaseReturnRepository;

    public GetPurchaseReturnListHandler(
        IPurchaseReturnRepository purchaseReturnRepository)
    {
        _purchaseReturnRepository = purchaseReturnRepository;
    }

    public Task<List<PurchaseReturnListItemDto>> Handle(
        GetPurchaseReturnListQuery request,
        CancellationToken cancellationToken)
    {
        var query = _purchaseReturnRepository.GetQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.DocumentNumber.Contains(request.Search) ||
                x.Supplier.Name.Contains(request.Search));
        }

        var result = query
            .OrderByDescending(x => x.DocumentDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new PurchaseReturnListItemDto
            {
                Id = x.Id,
                DocumentNumber = x.DocumentNumber,
                DocumentDate = x.DocumentDate,
                SupplierName = x.Supplier.Name,
                WarehouseName = x.Warehouse.Name,
                ReturnReason = x.ReturnReason,
                TotalAmount = x.TotalAmount,
                Status = x.Status.ToString()
            })
            .ToList();

        return Task.FromResult(result);
    }
}