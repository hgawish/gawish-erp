using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Infrastructure.Services;

public sealed class PurchasePostingService : IPurchasePostingService
{
    private readonly IPostingEngine _postingEngine;

    public PurchasePostingService(IPostingEngine postingEngine)
    {
        _postingEngine = postingEngine;
    }

    public async Task PostPurchaseInvoiceAsync(
        PurchaseHeader purchase,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(purchase);

        var context = new PostingContext
        {
            DocumentId = purchase.Id,
            DocumentType = DocumentType.Purchase,
            DocumentNumber = purchase.DocumentNumber,
            PostingDate = purchase.DocumentDate,
            FiscalYearId = purchase.FiscalYearId,
            CompanyId = purchase.CompanyId,
            BranchId = purchase.BranchId,
            ReferenceNumber = purchase.InvoiceNumber,
            Description = purchase.Notes,
            Amount = purchase.NetTotal,

            Lines = purchase.Lines.Select(x => new PostingLineContext
            {
                ProductId = x.ProductId,
                WarehouseId = purchase.WarehouseId,
                Quantity = x.Quantity,

                // PurchaseLine يحتوي على UnitCost وليس UnitPrice
                UnitPrice = x.UnitCost,

                BatchNumber = x.BatchNumber,
                ExpiryDate = x.ExpiryDate,
                Description = x.Notes
            }).ToList()
        };

        await _postingEngine.PostDocumentAsync(
            context,
            cancellationToken);
    }

    public async Task PostPurchaseReturnAsync(
        PurchaseReturnHeader purchaseReturn,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(purchaseReturn);

        var context = new PostingContext
        {
            DocumentId = purchaseReturn.Id,
            DocumentType = DocumentType.PurchaseReturn,
            DocumentNumber = purchaseReturn.DocumentNumber,
            PostingDate = purchaseReturn.DocumentDate,
            FiscalYearId = purchaseReturn.FiscalYearId,
            CompanyId = purchaseReturn.CompanyId,
            BranchId = purchaseReturn.BranchId,
            ReferenceNumber = purchaseReturn.DocumentNumber,
            Description = purchaseReturn.Notes,
            Amount = purchaseReturn.TotalAmount,

            Lines = purchaseReturn.Lines.Select(x => new PostingLineContext
            {
                ProductId = x.ProductId,
                WarehouseId = purchaseReturn.WarehouseId,
                Quantity = x.Quantity,

                // PurchaseReturnLine يحتوي على UnitCost
                UnitPrice = x.UnitCost,

                // هذه البيانات موجودة داخل PurchaseLine
                BatchNumber = x.PurchaseLine.BatchNumber,
                ExpiryDate = x.PurchaseLine.ExpiryDate,

                Description = x.Notes
            }).ToList()
        };

        await _postingEngine.PostDocumentAsync(
            context,
            cancellationToken);
    }

    public Task ReversePurchaseInvoiceAsync(
        Guid purchaseId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException(
            "ReversePurchaseInvoiceAsync will be implemented after Posting Engine V2 is completed.");
    }
}