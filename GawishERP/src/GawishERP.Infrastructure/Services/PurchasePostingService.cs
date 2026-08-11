using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Services;

public sealed class PurchasePostingService
    : IPurchasePostingService
{
    private readonly IPostingEngine _postingEngine;

    private readonly IInventoryService _inventoryService;

    private readonly IPurchaseRepository _purchaseRepository;

    private readonly ApplicationDbContext _context;

    public PurchasePostingService(
        IPostingEngine postingEngine,
        IInventoryService inventoryService,
        IPurchaseRepository purchaseRepository,
        ApplicationDbContext context)
    {
        _postingEngine = postingEngine;

        _inventoryService = inventoryService;

        _purchaseRepository = purchaseRepository;

        _context = context;
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

                                UnitPrice =
                                    x.UnitCost,

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

    public async Task ReversePurchaseInvoiceAsync(
        Guid purchaseId,
        CancellationToken cancellationToken = default)
    {
        if (purchaseId == Guid.Empty)
        {
            throw new ArgumentException(
                "Purchase ID is required.",
                nameof(purchaseId));
        }

        //=====================================================
        // Load Purchase
        //=====================================================

        var purchase =
            await _purchaseRepository
                .GetByIdWithLinesAsync(
                    purchaseId,
                    cancellationToken);

        if (purchase is null)
        {
            throw new InvalidOperationException(
                "Purchase document not found.");
        }

        //=====================================================
        // Validate Purchase Status
        //=====================================================

        if (purchase.Status !=
            DocumentStatus.Posted)
        {
            throw new InvalidOperationException(
                "Only posted purchases can be reversed.");
        }

        var lines =
            purchase.Lines.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Purchase document has no lines.");
        }

        //=====================================================
        // Detect Existing Inventory Reverse
        //=====================================================
        //
        // A reversed Purchase creates PurchaseReturn
        // stock transactions using the original Purchase Id
        // as ReferenceId.
        //
        // This protects the inventory side from being reversed
        // twice.
        //
        //=====================================================

        foreach (var line in lines)
        {
            var existingTransactions =
                await _context.StockTransactions
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.ReferenceId ==
                                purchase.Id &&

                            x.ProductId ==
                                line.ProductId &&

                            x.WarehouseId ==
                                purchase.WarehouseId &&

                            x.TransactionType ==
                                StockTransactionType.PurchaseReturn)
                    .ToListAsync(
                        cancellationToken);

            if (existingTransactions.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Purchase '{purchase.DocumentNumber}' " +
                    $"has already been reversed for product " +
                    $"'{line.ProductId}'.");
            }
        }

        //=====================================================
        // Database Transaction
        //=====================================================

        await using var transaction =
            await _context.Database
                .BeginTransactionAsync(
                    cancellationToken);

        try
        {
            //=================================================
            // Reverse Inventory
            //=================================================

            foreach (var line in lines)
            {
                var historicalUnitCost =
                    line.UnitCost;

                var inventoryResult =
                    await _inventoryService
                        .ReversePurchaseAsync(
                            productId:
                                line.ProductId,

                            warehouseId:
                                purchase.WarehouseId,

                            quantity:
                                line.Quantity,

                            unitCost:
                                historicalUnitCost,

                            transactionDate:
                                purchase.DocumentDate,

                            referenceId:
                                purchase.Id,

                            referenceNumber:
                                $"REV-{purchase.DocumentNumber}",

                            notes:
                                $"Reverse Purchase " +
                                $"{purchase.DocumentNumber}",

                            cancellationToken:
                                cancellationToken);

                if (inventoryResult.UnitCost !=
                    historicalUnitCost)
                {
                    throw new InvalidOperationException(
                        $"Historical cost mismatch while " +
                        $"reversing product " +
                        $"'{line.ProductId}'.");
                }
            }

            //=================================================
            // Reverse Accounting
            //=================================================

            var postingContext =
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
                        $"Reverse Purchase " +
                        $"{purchase.DocumentNumber}",

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

                                    UnitPrice =
                                        x.UnitCost,

                                    UnitCost =
                                        x.UnitCost,

                                    BatchNumber =
                                        x.BatchNumber,

                                    ExpiryDate =
                                        x.ExpiryDate,

                                    Description =
                                        $"Reverse Purchase Line"
                                })
                            .ToList()
                };

            await _postingEngine.ReverseDocumentAsync(
                postingContext,
                cancellationToken);

            //=================================================
            // Commit
            //=================================================

            await transaction.CommitAsync(
                cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}