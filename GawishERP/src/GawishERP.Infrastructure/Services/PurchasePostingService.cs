using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Infrastructure.Services;

public sealed class PurchasePostingService : IPurchasePostingService
{
    private readonly IPostingEngine _postingEngine;

    public PurchasePostingService(
        IPostingEngine postingEngine)
    {
        _postingEngine = postingEngine;
    }

    //=========================================================
    // Post Purchase Invoice
    //=========================================================

    public async Task PostPurchaseInvoiceAsync(
        PurchaseHeader purchase,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(purchase);

        var lines = purchase.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Purchase document has no lines.");
        }

        var context = new PostingContext
        {
            //=================================================
            // Document
            //=================================================

            DocumentId = purchase.Id,

            DocumentType = DocumentType.Purchase,

            DocumentNumber = purchase.DocumentNumber,

            PostingDate = purchase.DocumentDate,

            FiscalYearId = purchase.FiscalYearId,

            CompanyId = purchase.CompanyId,

            BranchId = purchase.BranchId,

            ReferenceNumber = purchase.InvoiceNumber,

            Description = purchase.Notes,

            //=================================================
            // Amounts
            //=================================================

            // Net amount including tax and after discount.
            Amount = purchase.NetTotal,

            // Gross purchase amount before discount.
            TotalBeforeDiscount = purchase.TotalBeforeDiscount,

            // Purchase discount.
            DiscountAmount = purchase.DiscountAmount,

            // Input VAT / purchase tax.
            TaxAmount = purchase.TaxAmount,

            // Inventory cost before discount.
            //
            // The discount can be posted separately through
            // PostingAmountSource.Discount.
            CostAmount = purchase.TotalBeforeDiscount,

            // Total quantity of purchased items.
            Quantity = lines.Sum(x => x.Quantity),

            //=================================================
            // Lines
            //=================================================

            Lines = lines
                .Select(x => new PostingLineContext
                {
                    ProductId = x.ProductId,

                    WarehouseId = purchase.WarehouseId,

                    Quantity = x.Quantity,

                    // PurchaseLine contains UnitCost.
                    UnitPrice = x.UnitCost,

                    // For a purchase document the purchase cost
                    // is the same value as UnitPrice.
                    UnitCost = x.UnitCost,

                    BatchNumber = x.BatchNumber,

                    ExpiryDate = x.ExpiryDate,

                    Description = x.Notes
                })
                .ToList()
        };

        await _postingEngine.PostDocumentAsync(
            context,
            cancellationToken);
    }

    //=========================================================
    // Post Purchase Return
    //=========================================================

    public async Task PostPurchaseReturnAsync(
        PurchaseReturnHeader purchaseReturn,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(purchaseReturn);

        var lines = purchaseReturn.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Purchase return document has no lines.");
        }

        var context = new PostingContext
        {
            //=================================================
            // Document
            //=================================================

            DocumentId = purchaseReturn.Id,

            DocumentType = DocumentType.PurchaseReturn,

            DocumentNumber = purchaseReturn.DocumentNumber,

            PostingDate = purchaseReturn.DocumentDate,

            FiscalYearId = purchaseReturn.FiscalYearId,

            CompanyId = purchaseReturn.CompanyId,

            BranchId = purchaseReturn.BranchId,

            ReferenceNumber = purchaseReturn.DocumentNumber,

            Description = purchaseReturn.Notes,

            //=================================================
            // Amounts
            //=================================================

            Amount = purchaseReturn.TotalAmount,

            TotalBeforeDiscount = purchaseReturn.TotalAmount,

            CostAmount = purchaseReturn.TotalAmount,

            Quantity = lines.Sum(x => x.Quantity),

            //=================================================
            // Lines
            //=================================================

            Lines = lines
                .Select(x => new PostingLineContext
                {
                    ProductId = x.ProductId,

                    WarehouseId = purchaseReturn.WarehouseId,

                    Quantity = x.Quantity,

                    // PurchaseReturnLine contains UnitCost.
                    UnitPrice = x.UnitCost,

                    UnitCost = x.UnitCost,

                    // PurchaseReturnLine itself does not contain
                    // batch/expiry information.
                    //
                    // The original PurchaseLine contains these
                    // values, so use it when the navigation is loaded.
                    BatchNumber = x.PurchaseLine.BatchNumber,

                    ExpiryDate = x.PurchaseLine.ExpiryDate,

                    Description = x.Notes
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
            "ReversePurchaseInvoiceAsync will be implemented after the document reversal workflow is completed.");
    }
}