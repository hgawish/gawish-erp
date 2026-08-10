using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public sealed class SalesReturnPostingService
    : ISalesReturnPostingService
{
    private readonly IPostingEngine _postingEngine;

    private readonly ISalesReturnRepository
        _salesReturnRepository;

    private readonly IStockTransactionRepository
        _stockTransactionRepository;

    private readonly IInventoryService
        _inventoryService;

    public SalesReturnPostingService(
        IPostingEngine postingEngine,
        ISalesReturnRepository salesReturnRepository,
        IStockTransactionRepository stockTransactionRepository,
        IInventoryService inventoryService)
    {
        _postingEngine = postingEngine;

        _salesReturnRepository =
            salesReturnRepository;

        _stockTransactionRepository =
            stockTransactionRepository;

        _inventoryService =
            inventoryService;
    }

    //=========================================================
    // Post Sales Return
    //=========================================================

    public async Task PostSalesReturnAsync(
        SalesReturnHeader salesReturn,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(salesReturn);

        var lines =
            salesReturn.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Sales return document has no lines.");
        }

        //=====================================================
        // Get Original Sale Transactions
        //=====================================================

        var originalSaleTransactions =
            await _stockTransactionRepository
                .GetByReferenceAsync(
                    salesReturn.SalesId,
                    StockTransactionType.Sale);

        if (originalSaleTransactions.Count == 0)
        {
            throw new InvalidOperationException(
                $"Original sale inventory transactions were " +
                $"not found for sales document " +
                $"'{salesReturn.SalesId}'.");
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
            // Find Original Sale Transaction
            //=================================================

            var originalTransaction =
                originalSaleTransactions
                    .FirstOrDefault(
                        x =>
                            x.ProductId ==
                                line.ProductId &&
                            x.WarehouseId ==
                                salesReturn.WarehouseId);

            if (originalTransaction is null)
            {
                throw new InvalidOperationException(
                    $"Original sale inventory transaction was " +
                    $"not found for product " +
                    $"'{line.ProductId}'.");
            }

            //=================================================
            // Historical Cost
            //=================================================

            var historicalUnitCost =
                originalTransaction.UnitCost;

            var lineCost =
                line.Quantity *
                historicalUnitCost;

            totalCost += lineCost;

            //=================================================
            // Return Inventory
            //=================================================
            //
            // Sales Return increases inventory using the
            // historical cost of the original sale.
            //
            // This is NOT the customer's selling price.
            //=================================================

            await _inventoryService.AddSalesReturnAsync(
                productId:
                    line.ProductId,

                warehouseId:
                    salesReturn.WarehouseId,

                quantity:
                    line.Quantity,

                unitCost:
                    historicalUnitCost,

                transactionDate:
                    salesReturn.DocumentDate,

                referenceId:
                    salesReturn.Id,

                referenceNumber:
                    salesReturn.DocumentNumber,

                notes:
                    line.Notes,

                cancellationToken:
                    cancellationToken);

            //=================================================
            // Posting Line
            //=================================================

            postingLines.Add(
                new PostingLineContext
                {
                    ProductId =
                        line.ProductId,

                    WarehouseId =
                        salesReturn.WarehouseId,

                    Quantity =
                        line.Quantity,

                    // Customer refund / return value.
                    UnitPrice =
                        line.UnitPrice,

                    // Historical inventory cost.
                    UnitCost =
                        historicalUnitCost,

                    BatchNumber =
                        line.SalesLine.BatchNumber,

                    ExpiryDate =
                        line.SalesLine.ExpiryDate,

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
                DocumentId =
                    salesReturn.Id,

                DocumentType =
                    DocumentType.SalesReturn,

                DocumentNumber =
                    salesReturn.DocumentNumber,

                PostingDate =
                    salesReturn.DocumentDate,

                FiscalYearId =
                    salesReturn.FiscalYearId,

                CompanyId =
                    salesReturn.CompanyId,

                BranchId =
                    salesReturn.BranchId,

                ReferenceNumber =
                    salesReturn.DocumentNumber,

                Description =
                    salesReturn.Notes,

                //=================================================
                // Sales Return Value
                //=================================================

                Amount =
                    salesReturn.TotalAmount,

                //=================================================
                // Historical Inventory Cost
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
    // Reverse Sales Return
    //=========================================================

    public async Task ReverseSalesReturnAsync(
        Guid salesReturnId,
        CancellationToken cancellationToken = default)
    {
        if (salesReturnId == Guid.Empty)
        {
            throw new ArgumentException(
                "Sales return ID is required.",
                nameof(salesReturnId));
        }

        var salesReturn =
            await _salesReturnRepository
                .GetByIdWithLinesAsync(
                    salesReturnId,
                    cancellationToken);

        if (salesReturn is null)
        {
            throw new InvalidOperationException(
                "Sales Return not found.");
        }

        var lines =
            salesReturn.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Sales return document has no lines.");
        }

        //=====================================================
        // Get Original Sales Return Transactions
        //=====================================================
        //
        // The original Sales Return created:
        //
        // TransactionType = SalesReturn
        // ReferenceId      = SalesReturn.Id
        //
        // UnitCost contains the historical cost that was
        // used when inventory was returned.
        //=====================================================

        var salesReturnTransactions =
            await _stockTransactionRepository
                .GetByReferenceAsync(
                    salesReturn.Id,
                    StockTransactionType.SalesReturn);

        if (salesReturnTransactions.Count == 0)
        {
            throw new InvalidOperationException(
                $"Sales return inventory transactions were " +
                $"not found for document " +
                $"'{salesReturn.DocumentNumber}'.");
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
            // Find Original Sales Return Transaction
            //=================================================

            var originalTransaction =
                salesReturnTransactions
                    .FirstOrDefault(
                        x =>
                            x.ProductId ==
                                line.ProductId &&
                            x.WarehouseId ==
                                salesReturn.WarehouseId);

            if (originalTransaction is null)
            {
                throw new InvalidOperationException(
                    $"Original sales return inventory " +
                    $"transaction was not found for product " +
                    $"'{line.ProductId}'.");
            }

            //=================================================
            // Historical Cost
            //=================================================

            var historicalUnitCost =
                originalTransaction.UnitCost;

            var lineCost =
                line.Quantity *
                historicalUnitCost;

            totalCost += lineCost;

            //=================================================
            // Reverse Inventory
            //=================================================
            //
            // Original Sales Return:
            //
            //     Inventory +
            //
            // Reverse:
            //
            //     Inventory -
            //
            // using the SAME historical cost.
            //=================================================

            await _inventoryService
                .ReverseSalesReturnAsync(
                    productId:
                        line.ProductId,

                    warehouseId:
                        salesReturn.WarehouseId,

                    quantity:
                        line.Quantity,

                    unitCost:
                        historicalUnitCost,

                    transactionDate:
                        salesReturn.DocumentDate,

                    referenceId:
                        salesReturn.Id,

                    referenceNumber:
                        salesReturn.DocumentNumber,

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
                        salesReturn.WarehouseId,

                    Quantity =
                        line.Quantity,

                    UnitPrice =
                        line.UnitPrice,

                    UnitCost =
                        historicalUnitCost,

                    BatchNumber =
                        line.SalesLine.BatchNumber,

                    ExpiryDate =
                        line.SalesLine.ExpiryDate,

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
                    salesReturn.Id,

                DocumentType =
                    DocumentType.SalesReturn,

                DocumentNumber =
                    salesReturn.DocumentNumber,

                PostingDate =
                    salesReturn.DocumentDate,

                FiscalYearId =
                    salesReturn.FiscalYearId,

                CompanyId =
                    salesReturn.CompanyId,

                BranchId =
                    salesReturn.BranchId,

                ReferenceNumber =
                    salesReturn.DocumentNumber,

                Description =
                    salesReturn.Notes,

                //=================================================
                // Return Value
                //=================================================

                Amount =
                    salesReturn.TotalAmount,

                //=================================================
                // Historical Inventory Cost
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