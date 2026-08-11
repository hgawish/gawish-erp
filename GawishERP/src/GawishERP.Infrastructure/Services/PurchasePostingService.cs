using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Infrastructure.Services;

public sealed class PurchasePostingService
    : IPurchasePostingService
{
    private readonly IPostingEngine _postingEngine;

    private readonly IInventoryService _inventoryService;

    public PurchasePostingService(
        IPostingEngine postingEngine,
        IInventoryService inventoryService)
    {
        _postingEngine = postingEngine;

        _inventoryService = inventoryService;
    }

    //=========================================================
    // Post Purchase Invoice
    //=========================================================

    public async Task PostPurchaseInvoiceAsync(
        PurchaseHeader purchase,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(purchase);

        var lines =
            purchase.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Purchase document has no lines.");
        }

        //=====================================================
        // Inventory
        //=====================================================
        //
        // Every Purchase Line creates:
        //
        //     StockTransaction.Purchase
        //
        // with:
        //
        //     UnitCost     = PurchaseLine.UnitCost
        //     ReferenceId  = PurchaseHeader.Id
        //
        // This transaction becomes the historical-cost source
        // for future Purchase Returns.
        //
        //=====================================================

        foreach (var line in lines)
        {
            await _inventoryService.AddPurchaseAsync(
                productId:
                    line.ProductId,

                warehouseId:
                    purchase.WarehouseId,

                quantity:
                    line.Quantity,

                unitCost:
                    line.UnitCost,

                transactionDate:
                    purchase.DocumentDate,

                referenceId:
                    purchase.Id,

                referenceNumber:
                    purchase.DocumentNumber,

                notes:
                    line.Notes,

                cancellationToken:
                    cancellationToken);
        }

        //=====================================================
        // Accounting Posting
        //=====================================================

        var context =
            new PostingContext
            {
                DocumentId =
                    purchase.Id,

                DocumentType =
                    DocumentType.Purchase,

                DocumentNumber =
                    purchase.DocumentNumber,

                PostingDate =
                    purchase.DocumentDate,

                FiscalYearId =
                    purchase.FiscalYearId,

                CompanyId =
                    purchase.CompanyId,

                BranchId =
                    purchase.BranchId,

                ReferenceNumber =
                    purchase.InvoiceNumber,

                Description =
                    purchase.Notes,

                Amount =
                    purchase.NetTotal,

                Lines =
                    lines.Select(
                        x =>
                            new PostingLineContext
                            {
                                ProductId =
                                    x.ProductId,

                                WarehouseId =
                                    purchase.WarehouseId,

                                Quantity =
                                    x.Quantity,

                                // PurchaseLine contains UnitCost.
                                UnitPrice =
                                    x.UnitCost,

                                // Historical purchase cost.
                                UnitCost =
                                    x.UnitCost,

                                BatchNumber =
                                    x.BatchNumber,

                                ExpiryDate =
                                    x.ExpiryDate,

                                Description =
                                    x.Notes
                            })
                        .ToList()
            };

        await _postingEngine.PostDocumentAsync(
            context,
            cancellationToken);
    }

    //=========================================================
    // Reverse Purchase Invoice
    //=========================================================

    public Task ReversePurchaseInvoiceAsync(
        Guid purchaseId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException(
            "ReversePurchaseInvoiceAsync will be implemented " +
            "after Purchase historical-cost posting is completed.");
    }
}