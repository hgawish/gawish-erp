using GawishERP.Application.Features.Purchasing.Purchase.Queries.GetById; using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Queries.GetById;

public sealed class GetPurchaseByIdHandler
    : IRequestHandler<GetPurchaseByIdQuery, PurchaseDetailsDto?>
{
    private readonly IPurchaseRepository _purchaseRepository;

    public GetPurchaseByIdHandler(
        IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<PurchaseDetailsDto?> Handle(
        GetPurchaseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdForViewAsync(
            request.Id,
            cancellationToken);

        if (purchase is null)
            return null;

        return new PurchaseDetailsDto
        {
            Id = purchase.Id,
            DocumentNumber = purchase.DocumentNumber,
            DocumentDate = purchase.DocumentDate,

            InvoiceNumber = purchase.InvoiceNumber,
            InvoiceDate = purchase.InvoiceDate,

            SupplierId = purchase.SupplierId,
            SupplierName = purchase.Supplier.Name,

            WarehouseId = purchase.WarehouseId,
            WarehouseName = purchase.Warehouse.Name,

            Currency = purchase.Currency,
            ExchangeRate = purchase.ExchangeRate,

            TotalBeforeDiscount = purchase.TotalBeforeDiscount,
            DiscountAmount = purchase.DiscountAmount,
            TaxAmount = purchase.TaxAmount,
            NetTotal = purchase.NetTotal,

            Status = purchase.Status.ToString(),
            Notes = purchase.Notes,

            Lines = purchase.Lines
                .Select(line => new PurchaseLineDto
                {
                    Id = line.Id,

                    ProductId = line.ProductId,
                    ProductCode = line.Product.Code,
                    ProductName = line.Product.Name,

                    Quantity = line.Quantity,
                    UnitCost = line.UnitCost,

                    DiscountAmount = line.DiscountAmount,
                    TaxAmount = line.TaxAmount,

                    LineTotal = line.LineTotal,

                    BatchNumber = line.BatchNumber,
                    ExpiryDate = line.ExpiryDate,

                    Notes = line.Notes
                })
                .ToList()
        };
    }
}