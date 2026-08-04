using GawishERP.Application.Common.Results;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Queries.GetList;

public sealed class GetPurchaseListQueryHandler
    : IRequestHandler<
        GetPurchaseListQuery,
        PagedResult<PurchaseListItemDto>>
{
    private readonly IPurchaseRepository _purchaseRepository;

    public GetPurchaseListQueryHandler(
        IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<PagedResult<PurchaseListItemDto>> Handle(
        GetPurchaseListQuery request,
        CancellationToken cancellationToken)
    {
        var purchases =
            await _purchaseRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.Search,
                request.SortBy,
                request.Descending,
                cancellationToken);

        var totalCount =
            await _purchaseRepository.CountAsync(
                request.Search,
                cancellationToken);

        var items = purchases
            .Select(p => new PurchaseListItemDto
            {
                Id = p.Id,
                DocumentNumber = p.DocumentNumber,
                DocumentDate = p.DocumentDate,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceDate = p.InvoiceDate,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier.Name,
                WarehouseId = p.WarehouseId,
                WarehouseName = p.Warehouse.Name,
                NetTotal = p.NetTotal,
                Status = p.Status.ToString(),
                LineCount = p.Lines.Count
            })
            .ToList();

        return PagedResult<PurchaseListItemDto>.Create(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}