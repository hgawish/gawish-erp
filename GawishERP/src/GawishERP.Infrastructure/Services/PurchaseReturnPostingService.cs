using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public sealed class PurchaseReturnPostingService
    : IPurchaseReturnPostingService
{
    private readonly IPostingEngine _postingEngine;

    private readonly IPurchaseReturnRepository
        _purchaseReturnRepository;

    private readonly IInventoryService
        _inventoryService;

    public PurchaseReturnPostingService(
        IPostingEngine postingEngine,
        IPurchaseReturnRepository purchaseReturnRepository,
        IInventoryService inventoryService)
    {
        _postingEngine = postingEngine;

        _purchaseReturnRepository =
            purchaseReturnRepository;

        _inventoryService =
            inventoryService;
    }

    //=========================================================
    // Post Purchase Return
    //=========================================================

    public async Task PostPurchaseReturnAsync(
        PurchaseReturnHeader purchaseReturn,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            purchaseReturn);

        var lines =
            purchaseReturn.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Purchase return document has no lines.");
        }

        //=====================================================
        // Validate Purchase Relationship
        //=====================================================

        foreach (var line in lines)
        {
            if (line.PurchaseLine is null)
            {
                throw new InvalidOperationException(
                    $"Purchase line '{line.PurchaseLineId}' " +
                    $"was not loaded for purchase return.");
            }

            //=================================================
            // Historical Cost
            //=================================================

            var historicalUnitCost =
                line.PurchaseLine.UnitCost;

            //=================================================
            // Document Snapshot Validation
            //=================================================
            //
            // PurchaseReturnLine.UnitCost should represent
            // the historical cost captured when the return
            // line was created.
            //
            // If it differs from the original PurchaseLine
            // cost, stop instead of silently posting an
            // incorrect inventory valuation.
            //
            //=================================================

            if (line.UnitCost != historicalUnitCost)
            {
                throw new InvalidOperationException(
                    $"Historical cost mismatch for product " +
                    $"'{line.ProductId}'. " +
                    $"Purchase cost: {historicalUnitCost}, " +
                    $"Purchase return cost: {line.UnitCost}.");
            }

            //=================================================
            // Inventory
            //=================================================

            var inventoryResult =
                await _inventoryService
                    .AddPurchaseReturnAsync(
                        productId:
                            line.ProductId,

                        warehouseId:
                            purchaseReturn.WarehouseId,

                        quantity:
                            line.Quantity,

                        unitCost:
                            historicalUnitCost,

                        transactionDate:
                            purchaseReturn.DocumentDate,

                        referenceId:
                            purchaseReturn.Id,

                        referenceNumber:
                            purchaseReturn.DocumentNumber,

                        notes:
                            line.Notes,

                        cancellationToken:
                            cancellationToken);

            //=================================================
            // Safety Check
            //=================================================

            if (inventoryResult.UnitCost !=
                historicalUnitCost)
            {
                throw new InvalidOperationException(
                    $"Inventory historical cost mismatch " +
                    $"for product '{line.ProductId}'.");
            }
        }

        //=====================================================
        // Accounting Posting
        //=====================================================

        var context =
            new PostingContext
            {
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

                Amount =
                    purchaseReturn.TotalAmount,

                CostAmount =
                    lines.Sum(
                        x =>
                            x.Quantity *
                            x.PurchaseLine.UnitCost),

                Quantity =
                    lines.Sum(
                        x =>
                            x.Quantity),

                Lines =
                    lines
                        .Select(
                            x =>
                                new PostingLineContext
                                {
                                    ProductId =
                                        x.ProductId,

                                    WarehouseId =
                                        purchaseReturn
                                            .WarehouseId,

                                    Quantity =
                                        x.Quantity,

                                    // Historical purchase cost.
                                    UnitPrice =
                                        x.PurchaseLine.UnitCost,

                                    UnitCost =
                                        x.PurchaseLine.UnitCost,

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
        // Reverse Inventory
        //=====================================================

        foreach (var line in lines)
        {
            if (line.PurchaseLine is null)
            {
                throw new InvalidOperationException(
                    $"Purchase line '{line.PurchaseLineId}' " +
                    $"was not loaded for purchase return.");
            }

            //=================================================
            // Historical Cost
            //=================================================

            var historicalUnitCost =
                line.PurchaseLine.UnitCost;

            //=================================================
            // Validate Stored Return Cost
            //=================================================

            if (line.UnitCost != historicalUnitCost)
            {
                throw new InvalidOperationException(
                    $"Historical cost mismatch for product " +
                    $"'{line.ProductId}'. " +
                    $"Purchase cost: {historicalUnitCost}, " +
                    $"Purchase return cost: {line.UnitCost}.");
            }

            //=================================================
            // Reverse Inventory
            //=================================================
            //
            // Original Purchase Return:
            //
            //     Inventory -
            //
            // Reverse:
            //
            //     Inventory +
            //
            // using the SAME historical cost.
            //
            //=================================================

            var inventoryResult =
                await _inventoryService
                    .ReversePurchaseReturnAsync(
                        productId:
                            line.ProductId,

                        warehouseId:
                            purchaseReturn.WarehouseId,

                        quantity:
                            line.Quantity,

                        unitCost:
                            historicalUnitCost,

                        transactionDate:
                            purchaseReturn.DocumentDate,

                        referenceId:
                            purchaseReturn.Id,

                        referenceNumber:
                            purchaseReturn.DocumentNumber,

                        notes:
                            line.Notes,

                        cancellationToken:
                            cancellationToken);

            if (inventoryResult.UnitCost !=
                historicalUnitCost)
            {
                throw new InvalidOperationException(
                    $"Inventory historical cost mismatch " +
                    $"during purchase return reversal for " +
                    $"product '{line.ProductId}'.");
            }
        }

        //=====================================================
        // Reverse Accounting Posting
        //=====================================================

        var context =
            new PostingContext
            {
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

                Amount =
                    purchaseReturn.TotalAmount,

                CostAmount =
                    lines.Sum(
                        x =>
                            x.Quantity *
                            x.PurchaseLine.UnitCost),

                Quantity =
                    lines.Sum(
                        x =>
                            x.Quantity),

                Lines =
                    lines
                        .Select(
                            x =>
                                new PostingLineContext
                                {
                                    ProductId =
                                        x.ProductId,

                                    WarehouseId =
                                        purchaseReturn
                                            .WarehouseId,

                                    Quantity =
                                        x.Quantity,

                                    UnitPrice =
                                        x.PurchaseLine.UnitCost,

                                    UnitCost =
                                        x.PurchaseLine.UnitCost,

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