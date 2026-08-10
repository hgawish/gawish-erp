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
        ArgumentNullException.ThrowIfNull(
            salesReturn);

        var lines =
            salesReturn.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Sales return document has no lines.");
        }

        //=====================================================
        // Get Original Sale Stock Transactions
        //=====================================================
        //
        // The original Sales Invoice created StockTransaction
        // records using:
        //
        // ReferenceId = SalesHeader.Id
        // TransactionType = Sale
        //
        // Those transactions contain the historical UnitCost.
        //
        // We MUST NOT use:
        //
        // SalesLine.UnitPrice
        //
        // because UnitPrice is the customer's selling price.
        //
        //=====================================================

        var originalSaleTransactions =
            await _stockTransactionRepository
                .GetByReferenceAsync(
                    salesReturn.SalesId,
                    StockTransactionType.Sale);

        if (originalSaleTransactions.Count == 0)
        {
            throw new InvalidOperationException(
                $"Original sales inventory transactions " +
                $"were not found for sales document " +
                $"'{salesReturn.SalesId}'.");
        }

        //=====================================================
        // Build Posting Lines
        //=====================================================

        var postingLines =
            new List<PostingLineContext>(
                lines.Count);

        decimal totalCost = 0m;

        foreach (var line in lines)
        {
            //=================================================
            // Find Original Sale Cost
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
                    $"Original sales stock transaction " +
                    $"was not found for product " +
                    $"'{line.ProductId}'.");
            }

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
            // The stock is increased using the SAME
            // historical cost that was used when the
            // original sale reduced inventory.
            //
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

                    // Sales return selling price.
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
                //=================================================
                // Document
                //=================================================

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
                // Amounts
                //=================================================

                Amount =
                    salesReturn.TotalAmount,

                CostAmount =
                    totalCost,

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
        // Get Historical Sales Return Transactions
        //=====================================================
        //
        // When the Sales Return was posted, InventoryService
        // created StockTransaction records with:
        //
        // ReferenceId = SalesReturn.Id
        // TransactionType = SalesReturn
        //
        // Those records contain the exact UnitCost used
        // when the stock was increased.
        //
        //=====================================================

        var salesReturnTransactions =
            await _stockTransactionRepository
                .GetByReferenceAsync(
                    salesReturn.Id,
                    StockTransactionType.SalesReturn);

        if (salesReturnTransactions.Count == 0)
        {
            throw new InvalidOperationException(
                $"Sales return inventory transactions " +
                $"were not found for document " +
                $"'{salesReturn.Id}'.");
        }

        //=====================================================
        // Build Reverse Posting Lines
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
                    $"Original sales return stock " +
                    $"transaction was not found for " +
                    $"product '{line.ProductId}'.");
            }

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
            // The original Sales Return increased stock.
            //
            // Reversing it must decrease stock using the
            // SAME historical cost.
            //
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
                //=================================================
                // Document
                //=================================================

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
                // Amounts
                //=================================================

                Amount =
                    salesReturn.TotalAmount,

                CostAmount =
                    totalCost,

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