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

    public SalesPostingService(
        IPostingEngine postingEngine,
        ISalesRepository salesRepository,
        IInventoryService inventoryService)
    {
        _postingEngine = postingEngine;
        _salesRepository = salesRepository;
        _inventoryService = inventoryService;
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
        //
        // For every sales line:
        //
        // 1. Read current AverageCost from InventoryBalance.
        // 2. Decrease stock.
        // 3. Capture actual UnitCost.
        // 4. Capture actual TotalCost.
        //
        // The supplied UnitPrice is NEVER used as inventory cost.
        //=====================================================

        var postingLines = new List<PostingLineContext>(
            lines.Count);

        decimal totalCost = 0m;

        foreach (var line in lines)
        {
            var inventoryResult =
                await _inventoryService.AddSaleAsync(
                    productId: line.ProductId,
                    warehouseId: sales.WarehouseId,
                    quantity: line.Quantity,
                    unitCost: 0m,
                    transactionDate: sales.DocumentDate,
                    referenceId: sales.Id,
                    referenceNumber: sales.DocumentNumber,
                    notes: line.Notes,
                    cancellationToken: cancellationToken);

            totalCost += inventoryResult.TotalCost;

            postingLines.Add(
                new PostingLineContext
                {
                    ProductId = line.ProductId,

                    WarehouseId = sales.WarehouseId,

                    Quantity = line.Quantity,

                    // Customer selling price.
                    UnitPrice = line.UnitPrice,

                    // Actual inventory cost returned by InventoryService.
                    UnitCost = inventoryResult.UnitCost,

                    BatchNumber = line.BatchNumber,

                    ExpiryDate = line.ExpiryDate,

                    Description = line.Notes
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

            DocumentId = sales.Id,

            DocumentType = DocumentType.Sales,

            DocumentNumber = sales.DocumentNumber,

            PostingDate = sales.DocumentDate,

            FiscalYearId = sales.FiscalYearId,

            CompanyId = sales.CompanyId,

            BranchId = sales.BranchId,

            ReferenceNumber = sales.DocumentNumber,

            Description = sales.Notes,

            //=================================================
            // Sales Amounts
            //=================================================

            Amount = sales.NetTotal,

            TotalBeforeDiscount =
                sales.TotalBeforeDiscount,

            DiscountAmount =
                sales.DiscountAmount,

            TaxAmount =
                sales.TaxAmount,

            //=================================================
            // Actual Inventory Cost
            //=================================================

            CostAmount = totalCost,

            //=================================================
            // Quantity
            //=================================================

            Quantity =
                lines.Sum(x => x.Quantity),

            //=================================================
            // Posting Lines
            //=================================================

            Lines = postingLines
        };

        //=====================================================
        // Accounting Posting
        //=====================================================

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

        var lines = sales.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Sales document has no lines.");
        }

        //=====================================================
        // IMPORTANT
        //=====================================================
        //
        // We intentionally do NOT attempt to calculate the
        // historical inventory cost here yet.
        //
        // The original cost must come from the stock transaction
        // created when the invoice was posted.
        //
        // That historical-cost lookup will be implemented in
        // the dedicated reversal phase.
        //=====================================================

        var context = new PostingContext
        {
            //=================================================
            // Document
            //=================================================

            DocumentId = sales.Id,

            DocumentType = DocumentType.Sales,

            DocumentNumber = sales.DocumentNumber,

            PostingDate = sales.DocumentDate,

            FiscalYearId = sales.FiscalYearId,

            CompanyId = sales.CompanyId,

            BranchId = sales.BranchId,

            ReferenceNumber = sales.DocumentNumber,

            Description = sales.Notes,

            //=================================================
            // Sales Amounts
            //=================================================

            Amount = sales.NetTotal,

            TotalBeforeDiscount =
                sales.TotalBeforeDiscount,

            DiscountAmount =
                sales.DiscountAmount,

            TaxAmount =
                sales.TaxAmount,

            //=================================================
            // Historical Cost
            //=================================================
            //
            // Not available from SalesLine itself.
            // Will be resolved from the original inventory
            // transaction during the reversal phase.
            //

            CostAmount = 0m,

            Quantity =
                lines.Sum(x => x.Quantity),

            //=================================================
            // Lines
            //=================================================

            Lines = lines
                .Select(x =>
                    new PostingLineContext
                    {
                        ProductId = x.ProductId,

                        WarehouseId =
                            sales.WarehouseId,

                        Quantity =
                            x.Quantity,

                        UnitPrice =
                            x.UnitPrice,

                        UnitCost = 0m,

                        BatchNumber =
                            x.BatchNumber,

                        ExpiryDate =
                            x.ExpiryDate,

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