using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public sealed class SalesPostingService : ISalesPostingService
{
    private readonly IPostingEngine _postingEngine;

    private readonly ISalesRepository _salesRepository;

    private readonly IInventoryService _inventoryService;

    private readonly IStockTransactionRepository
        _stockTransactionRepository;

    public SalesPostingService(
        IPostingEngine postingEngine,
        ISalesRepository salesRepository,
        IInventoryService inventoryService,
        IStockTransactionRepository stockTransactionRepository)
    {
        _postingEngine = postingEngine;

        _salesRepository = salesRepository;

        _inventoryService = inventoryService;

        _stockTransactionRepository =
            stockTransactionRepository;
    }

    //=========================================================
    // Post Sales Invoice
    //=========================================================

    public async Task PostSalesInvoiceAsync(
        SalesHeader sales,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sales);

        var lines = sales.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Sales document has no lines.");
        }

        //=====================================================
        // Inventory Costing
        //=====================================================

        var postingLines =
            new List<PostingLineContext>(lines.Count);

        decimal totalCost = 0m;

        foreach (var line in lines)
        {
            var inventoryResult =
                await _inventoryService.AddSaleAsync(
                    productId:
                        line.ProductId,

                    warehouseId:
                        sales.WarehouseId,

                    quantity:
                        line.Quantity,

                    unitCost:
                        0m,

                    transactionDate:
                        sales.DocumentDate,

                    referenceId:
                        sales.Id,

                    referenceNumber:
                        sales.DocumentNumber,

                    notes:
                        line.Notes,

                    cancellationToken:
                        cancellationToken);

            totalCost += inventoryResult.TotalCost;

            postingLines.Add(
                new PostingLineContext
                {
                    ProductId =
                        line.ProductId,

                    WarehouseId =
                        sales.WarehouseId,

                    Quantity =
                        line.Quantity,

                    // Customer selling price.
                    UnitPrice =
                        line.UnitPrice,

                    // Actual inventory cost.
                    UnitCost =
                        inventoryResult.UnitCost,

                    BatchNumber =
                        line.BatchNumber,

                    ExpiryDate =
                        line.ExpiryDate,

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
                    sales.Id,

                DocumentType =
                    DocumentType.Sales,

                DocumentNumber =
                    sales.DocumentNumber,

                PostingDate =
                    sales.DocumentDate,

                FiscalYearId =
                    sales.FiscalYearId,

                CompanyId =
                    sales.CompanyId,

                BranchId =
                    sales.BranchId,

                ReferenceNumber =
                    sales.DocumentNumber,

                Description =
                    sales.Notes,

                //=================================================
                // Sales Amounts
                //=================================================

                Amount =
                    sales.NetTotal,

                TotalBeforeDiscount =
                    sales.TotalBeforeDiscount,

                DiscountAmount =
                    sales.DiscountAmount,

                TaxAmount =
                    sales.TaxAmount,

                //=================================================
                // Actual Inventory Cost
                //=================================================

                CostAmount =
                    totalCost,

                //=================================================
                // Quantity
                //=================================================

                Quantity =
                    lines.Sum(x => x.Quantity),

                //=================================================
                // Posting Lines
                //=================================================

                Lines =
                    postingLines
            };

        await _postingEngine.PostDocumentAsync(
            context,
            cancellationToken);
    }

    //=========================================================
    // Reverse Sales Invoice
    //=========================================================

    public async Task ReverseSalesInvoiceAsync(
        Guid salesId,
        CancellationToken cancellationToken = default)
    {
        if (salesId == Guid.Empty)
        {
            throw new ArgumentException(
                "Sales ID is required.",
                nameof(salesId));
        }

        var sales =
            await _salesRepository.GetByIdWithLinesAsync(
                salesId,
                cancellationToken);

        if (sales is null)
        {
            throw new InvalidOperationException(
                "Sales Invoice not found.");
        }

        var lines =
            sales.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Sales document has no lines.");
        }

        //=====================================================
        // Get Original Sale Stock Transactions
        //=====================================================
        //
        // The original sale created StockTransaction records
        // with:
        //
        // ReferenceId      = Sales.Id
        // TransactionType = Sale
        // UnitCost         = Historical inventory cost
        //
        // We use these records instead of SalesLine.UnitPrice.
        //=====================================================

        var originalTransactions =
            await _stockTransactionRepository
                .GetByReferenceAsync(
                    sales.Id,
                    StockTransactionType.Sale);

        if (originalTransactions.Count == 0)
        {
            throw new InvalidOperationException(
                $"Original inventory transactions were not " +
                $"found for sales invoice '{sales.DocumentNumber}'.");
        }

        //=====================================================
        // Build Reverse Posting Lines
        //=====================================================

        var postingLines =
            new List<PostingLineContext>(lines.Count);

        decimal totalCost = 0m;

        foreach (var line in lines)
        {
            //=================================================
            // Find Original Transaction
            //=================================================

            var originalTransaction =
                originalTransactions.FirstOrDefault(
                    x =>
                        x.ProductId ==
                            line.ProductId &&

                        x.WarehouseId ==
                            sales.WarehouseId);

            if (originalTransaction is null)
            {
                throw new InvalidOperationException(
                    $"Original inventory transaction was not " +
                    $"found for product '{line.ProductId}' " +
                    $"in warehouse '{sales.WarehouseId}'.");
            }

            //=================================================
            // Historical Cost
            //=================================================

            var historicalUnitCost =
                originalTransaction.UnitCost;

            var historicalLineCost =
                line.Quantity *
                historicalUnitCost;

            totalCost +=
                historicalLineCost;

            //=================================================
            // Reverse Inventory
            //=================================================
            //
            // Original Sale:
            //
            //     Stock -
            //
            // Reverse Sale:
            //
            //     Stock +
            //
            // Both use the SAME historical cost.
            //=================================================

            await _inventoryService.ReverseSaleAsync(
                productId:
                    line.ProductId,

                warehouseId:
                    sales.WarehouseId,

                quantity:
                    line.Quantity,

                unitCost:
                    historicalUnitCost,

                transactionDate:
                    sales.DocumentDate,

                referenceId:
                    sales.Id,

                referenceNumber:
                    sales.DocumentNumber,

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
                        sales.WarehouseId,

                    Quantity =
                        line.Quantity,

                    UnitPrice =
                        line.UnitPrice,

                    UnitCost =
                        historicalUnitCost,

                    BatchNumber =
                        line.BatchNumber,

                    ExpiryDate =
                        line.ExpiryDate,

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
                    sales.Id,

                DocumentType =
                    DocumentType.Sales,

                DocumentNumber =
                    sales.DocumentNumber,

                PostingDate =
                    sales.DocumentDate,

                FiscalYearId =
                    sales.FiscalYearId,

                CompanyId =
                    sales.CompanyId,

                BranchId =
                    sales.BranchId,

                ReferenceNumber =
                    sales.DocumentNumber,

                Description =
                    sales.Notes,

                //=================================================
                // Original Sales Amount
                //=================================================

                Amount =
                    sales.NetTotal,

                TotalBeforeDiscount =
                    sales.TotalBeforeDiscount,

                DiscountAmount =
                    sales.DiscountAmount,

                TaxAmount =
                    sales.TaxAmount,

                //=================================================
                // Historical Cost
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
        // Reverse Accounting Posting
        //=====================================================

        await _postingEngine.ReverseDocumentAsync(
            context,
            cancellationToken);
    }
}