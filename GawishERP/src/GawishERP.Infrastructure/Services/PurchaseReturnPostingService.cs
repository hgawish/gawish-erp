using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Application.Common.Inventory;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public sealed class PurchaseReturnPostingService
    : IPurchaseReturnPostingService
{
    private readonly IPostingEngine _postingEngine;
    private readonly IPurchaseReturnRepository _purchaseReturnRepository;
    private readonly IInventoryService _inventoryService;

    public PurchaseReturnPostingService(
        IPostingEngine postingEngine,
        IPurchaseReturnRepository purchaseReturnRepository,
        IInventoryService inventoryService)
    {
        _postingEngine = postingEngine;
        _purchaseReturnRepository = purchaseReturnRepository;
        _inventoryService = inventoryService;
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

        //=====================================================
        // Inventory
        //=====================================================
        //
        // Purchase Return:
        //
        // 1. Decrease inventory.
        // 2. InventoryService determines the actual inventory
        //    cost used for the stock movement.
        // 3. Capture that cost for accounting.
        //
        //=====================================================

        var postingLines =
            new List<PostingLineContext>(lines.Count);

        decimal totalCost = 0m;

        foreach (var line in lines)
        {
            var inventoryResult =
                await _inventoryService.AddPurchaseReturnAsync(
                    productId: line.ProductId,
                    warehouseId: purchaseReturn.WarehouseId,
                    quantity: line.Quantity,
                    unitCost: line.UnitCost,
                    transactionDate: purchaseReturn.DocumentDate,
                    referenceId: purchaseReturn.Id,
                    referenceNumber: purchaseReturn.DocumentNumber,
                    notes: line.Notes,
                    cancellationToken: cancellationToken);

            totalCost += inventoryResult.TotalCost;

            postingLines.Add(
                new PostingLineContext
                {
                    ProductId = line.ProductId,

                    WarehouseId =
                        purchaseReturn.WarehouseId,

                    Quantity =
                        line.Quantity,

                    // Amount of the purchase return line.
                    UnitPrice =
                        line.UnitCost,

                    // Actual inventory cost used by InventoryService.
                    UnitCost =
                        inventoryResult.UnitCost,

                    Description =
                        line.Notes
                });
        }

        //=====================================================
        // Posting Context
        //=====================================================

        var context = new PostingContext
        {
            //=================================================
            // Document
            //=================================================

            DocumentId =
                purchaseReturn.Id,

            DocumentType =
                DocumentType.PurchaseReturn,

            DocumentNumber =
                purchaseReturn.DocumentNumber,

            PostingDate =
                purchaseReturn.DocumentDate,

            FiscalYearId =
                purchaseReturn.FiscalYearId,

            CompanyId =
                purchaseReturn.CompanyId,

            BranchId =
                purchaseReturn.BranchId,

            ReferenceNumber =
                purchaseReturn.DocumentNumber,

            Description =
                purchaseReturn.Notes,

            //=================================================
            // Amount
            //=================================================

            Amount =
                purchaseReturn.TotalAmount,

            //=================================================
            // Cost
            //=================================================

            CostAmount =
                totalCost,

            //=================================================
            // Quantity
            //=================================================

            Quantity =
                lines.Sum(x => x.Quantity),

            //=================================================
            // Lines
            //=================================================

            Lines =
                postingLines
        };

        //=====================================================
        // Accounting Posting
        //=====================================================

        await _postingEngine.PostDocumentAsync(
            context,
            cancellationToken);
    }

    //=========================================================
    // Reverse Purchase Return
    //=========================================================

    public async Task ReversePurchaseReturnAsync(
        Guid purchaseReturnId,
        CancellationToken cancellationToken = default)
    {
        if (purchaseReturnId == Guid.Empty)
        {
            throw new ArgumentException(
                "Purchase return ID is required.",
                nameof(purchaseReturnId));
        }

        var purchaseReturn =
            await _purchaseReturnRepository
                .GetByIdWithLinesAsync(
                    purchaseReturnId,
                    cancellationToken);

        if (purchaseReturn is null)
        {
            throw new InvalidOperationException(
                "Purchase Return not found.");
        }

        var lines =
            purchaseReturn.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Purchase return document has no lines.");
        }

        //=====================================================
        // IMPORTANT
        //=====================================================
        //
        // The historical inventory cost used when the original
        // Purchase Return was posted must be restored here.
        //
        // InventoryService.ReversePurchaseAsync currently
        // performs a stock decrease using the current inventory
        // balance, so it is NOT sufficient for a true historical
        // reversal yet.
        //
        // Therefore we do NOT silently invent a historical cost.
        //
        // This reversal will be completed in the dedicated
        // Historical Inventory Cost / Reversal phase.
        //
        //=====================================================

        var context =
            new PostingContext
            {
                //=================================================
                // Document
                //=================================================

                DocumentId =
                    purchaseReturn.Id,

                DocumentType =
                    DocumentType.PurchaseReturn,

                DocumentNumber =
                    purchaseReturn.DocumentNumber,

                PostingDate =
                    purchaseReturn.DocumentDate,

                FiscalYearId =
                    purchaseReturn.FiscalYearId,

                CompanyId =
                    purchaseReturn.CompanyId,

                BranchId =
                    purchaseReturn.BranchId,

                ReferenceNumber =
                    purchaseReturn.DocumentNumber,

                Description =
                    purchaseReturn.Notes,

                //=================================================
                // Amount
                //=================================================

                Amount =
                    purchaseReturn.TotalAmount,

                //=================================================
                // Cost
                //=================================================

                CostAmount = 0m,

                //=================================================
                // Quantity
                //=================================================

                Quantity =
                    lines.Sum(x => x.Quantity),

                //=================================================
                // Lines
                //=================================================

                Lines =
                    lines
                        .Select(
                            x =>
                                new PostingLineContext
                                {
                                    ProductId =
                                        x.ProductId,

                                    WarehouseId =
                                        purchaseReturn.WarehouseId,

                                    Quantity =
                                        x.Quantity,

                                    UnitPrice =
                                        x.UnitCost,

                                    UnitCost = 0m,

                                    Description =
                                        x.Notes
                                })
                        .ToList()
            };

        await _postingEngine.ReverseDocumentAsync(
            context,
            cancellationToken);
    }
}