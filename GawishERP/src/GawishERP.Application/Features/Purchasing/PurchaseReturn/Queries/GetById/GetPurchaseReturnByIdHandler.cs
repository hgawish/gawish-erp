using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Queries.GetById;

public sealed class GetPurchaseReturnByIdHandler
    : IRequestHandler<GetPurchaseReturnByIdQuery, PurchaseReturnDetailsDto?>
{
    private readonly IPurchaseReturnRepository _purchaseReturnRepository;

    public GetPurchaseReturnByIdHandler(
        IPurchaseReturnRepository purchaseReturnRepository)
    {
        _purchaseReturnRepository = purchaseReturnRepository;
    }

    public async Task<PurchaseReturnDetailsDto?> Handle(
        GetPurchaseReturnByIdQuery request,
        CancellationToken cancellationToken)
    {
        var purchaseReturn =
            await _purchaseReturnRepository.GetByIdForViewAsync(
                request.Id,
                cancellationToken);

        if (purchaseReturn is null)
            return null;

        return new PurchaseReturnDetailsDto
        {
            Id = purchaseReturn.Id,

            DocumentNumber = purchaseReturn.DocumentNumber,

            DocumentDate = purchaseReturn.DocumentDate,

            PurchaseId = purchaseReturn.PurchaseId,

            SupplierId = purchaseReturn.SupplierId,

            SupplierName = purchaseReturn.Supplier.Name,

            WarehouseId = purchaseReturn.WarehouseId,

            WarehouseName = purchaseReturn.Warehouse.Name,

            ReturnReason = purchaseReturn.ReturnReason,

            TotalAmount = purchaseReturn.TotalAmount,

            Status = purchaseReturn.Status.ToString(),

            Notes = purchaseReturn.Notes,

            Lines = purchaseReturn.Lines
                .Select(l => new PurchaseReturnLineDto
                {
                    Id = l.Id,

                    PurchaseLineId = l.PurchaseLineId,

                    ProductId = l.ProductId,

                    ProductCode = l.Product.Code,

                    ProductName = l.Product.Name,

                    Quantity = l.Quantity,

                    UnitCost = l.UnitCost,

                    LineTotal = l.LineTotal,

                    Notes = l.Notes
                })
                .ToList()
        };
    }
}