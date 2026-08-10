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

    private readonly IStockTransactionRepository
        _stockTransactionRepository;

    public PurchaseReturnPostingService(
        IPostingEngine postingEngine,
        IPurchaseReturnRepository purchaseReturnRepository,
        IInventoryService inventoryService,
        IStockTransactionRepository stockTransactionRepository)
    {
        _postingEngine = postingEngine;

        _purchaseReturnRepository =
            purchaseReturnRepository;

        _inventoryService =
            inventoryService;

        _stockTransactionRepository =
            stockTransactionRepository;
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
        // Get Original Purchase Transactions
        //=====================================================
        //
        // The original Purchase created:
        //
        // ReferenceId      = Purchase.Id
        // TransactionType = Purchase
        // UnitCost         = historical purchase cost
        //
        // We use that cost instead of the current inventory
        // AverageCost.
        //
        //=====================================================

        var originalPurchaseTransactions =
            await _stockTransactionRepository
                .GetByReferenceAsync(
                    purchaseReturn.PurchaseId,
                    StockTransactionType.Purchase);

        if (originalPurchaseTransactions.Count == 0)
        {
            throw new InvalidOperationException(
                $"Original purchase inventory transactions " +
                $"were not found for purchase document " +
                $"'{purchaseReturn.PurchaseId}'.");
        }

        //=====================================================
        // Posting Lines
        //=====================================================

        var postingLines =
            new List<PostingLineContext>(
                lines.Count);

        decimal totalCost = 0m;

        foreach (var line in lines)
        {
            //=================================================
            // Find Historical Purchase Cost
            //=================================================

            var matchingTransactions =
                originalPurchaseTransactions
                    .Where(
                        x =>
                            x.ProductId ==
                                line.ProductId &&
                            x.WarehouseId ==
                                purchaseReturn.WarehouseId)
                    .ToList();

            if (matchingTransactions.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Original purchase inventory transaction " +
                    $"was not found for product " +
                    $"'{line.ProductId}'.");
            }

            if (matchingTransactions.Count > 1)
            {
                throw new InvalidOperationException(
                    $"Multiple original purchase inventory " +
                    $"transactions were found for product " +
                    $"'{line.ProductId}'. " +
                    $"Historical cost cannot be determined " +
                    $"unambiguously.");
            }

            var originalTransaction =
                matchingTransactions[0];

            var historicalUnitCost =
                originalTransaction.UnitCost;

            var lineCost =
                line.Quantity *
                historicalUnitCost;

            totalCost +=
                lineCost;

            //=================================================
            // Decrease Inventory
            //=================================================
            //
            // Purchase Return:
            //
            // Inventory -
            //
            // using the historical purchase cost.
            //
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
                    $"Historical purchase cost mismatch for " +
                    $"product '{line.ProductId}'.");
            }

            //=================================================
            // Posting Line
            //=================================================

            postingLines.Add(
                new PostingLineContext
                {
                    ProductId =
                        line.ProductId,

                    WarehouseId =
                        purchaseReturn.WarehouseId,

                    Quantity =
                        line.Quantity,

                    // Purchase return value.
                    UnitPrice =
                        line.UnitCost,

                    // Historical inventory cost.
                    UnitCost =
                        historicalUnitCost,

                    Description =
                        line.Notes
                });
        }

        //=====================================================
        // Posting Context
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
                // Historical Cost
                //=================================================

                CostAmount =
                    totalCost,

                //=================================================
                // Quantity
                //=================================================

                Quantity =
                    lines.Sum(
                        x => x.Quantity),

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
        // Get Original Purchase Return Transactions
        //=====================================================
        //
        // The original Purchase Return created:
        //
        // ReferenceId      = PurchaseReturn.Id
        // TransactionType = PurchaseReturn
        // UnitCost         = historical purchase cost
        //
        //=====================================================

        var originalReturnTransactions =
            await _stockTransactionRepository
                .GetByReferenceAsync(
                    purchaseReturn.Id,
                    StockTransactionType.PurchaseReturn);

        if (originalReturnTransactions.Count == 0)
        {
            throw new InvalidOperationException(
                $"Original purchase return inventory " +
                $"transactions were not found for document " +
                $"'{purchaseReturn.DocumentNumber}'.");
        }

        //=====================================================
        // Posting Lines
        //=====================================================

        var postingLines =
            new List<PostingLineContext>(
                lines.Count);

        decimal totalCost = 0m;

        foreach (var line in lines)
        {
            //=================================================
            // Find Historical Return Cost
            //=================================================

            var matchingTransactions =
                originalReturnTransactions
                    .Where(
                        x =>
                            x.ProductId ==
                                line.ProductId &&
                            x.WarehouseId ==
                                purchaseReturn.WarehouseId)
                    .ToList();

            if (matchingTransactions.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Original purchase return inventory " +
                    $"transaction was not found for product " +
                    $"'{line.ProductId}'.");
            }

            if (matchingTransactions.Count > 1)
            {
                throw new InvalidOperationException(
                    $"Multiple original purchase return " +
                    $"inventory transactions were found for " +
                    $"product '{line.ProductId}'. " +
                    $"Historical cost cannot be determined " +
                    $"unambiguously.");
            }

            var originalTransaction =
                matchingTransactions[0];

            var historicalUnitCost =
                originalTransaction.UnitCost;

            var lineCost =
                line.Quantity *
                historicalUnitCost;

            totalCost +=
                lineCost;

            //=================================================
            // Reverse Inventory
            //=================================================
            //
            // Original Purchase Return:
            //
            // Inventory -
            //
            // Reverse:
            //
            // Inventory +
            //
            // using the same historical cost.
            //
            //=================================================

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

            //=================================================
            // Reverse Posting Line
            //=================================================

            postingLines.Add(
                new PostingLineContext
                {
                    ProductId =
                        line.ProductId,

                    WarehouseId =
                        purchaseReturn.WarehouseId,

                    Quantity =
                        line.Quantity,

                    UnitPrice =
                        line.UnitCost,

                    UnitCost =
                        historicalUnitCost,

                    Description =
                        line.Notes
                });
        }

        //=====================================================
        // Reverse Posting Context
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

                //=================================================
                // Return Amount
                //=================================================

                Amount =
                    purchaseReturn.TotalAmount,

                //=================================================
                // Historical Cost
                //=================================================

                CostAmount =
                    totalCost,

                //=================================================
                // Quantity
                //=================================================

                Quantity =
                    lines.Sum(
                        x => x.Quantity),

                //=================================================
                // Lines
                //=================================================

                Lines =
                    postingLines
            };

        //=====================================================
        // Reverse Accounting Posting
        //=====================================================

        await _postingEngine.ReverseDocumentAsync(
            context,
            cancellationToken);
    }
}