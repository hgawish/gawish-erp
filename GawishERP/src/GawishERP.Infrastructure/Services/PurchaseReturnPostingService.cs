using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public sealed class PurchaseReturnPostingService : IPurchaseReturnPostingService
{
    private readonly IPostingEngine _postingEngine;
    private readonly IPurchaseReturnRepository _purchaseReturnRepository;

    public PurchaseReturnPostingService(
        IPostingEngine postingEngine,
        IPurchaseReturnRepository purchaseReturnRepository)
    {
        _postingEngine = postingEngine;
        _purchaseReturnRepository = purchaseReturnRepository;
    }

    public async Task PostPurchaseReturnAsync(
        PurchaseReturnHeader purchaseReturn,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(purchaseReturn);

        var context = new PostingContext
        {
            DocumentId = purchaseReturn.Id,
            DocumentType = DocumentType.PurchaseReturn,
            DocumentNumber = purchaseReturn.DocumentNumber,
            PostingDate = purchaseReturn.DocumentDate,
            FiscalYearId = purchaseReturn.FiscalYearId,
            CompanyId = purchaseReturn.CompanyId,
            BranchId = purchaseReturn.BranchId,
            ReferenceNumber = purchaseReturn.DocumentNumber,
            Description = purchaseReturn.Notes,
            Amount = purchaseReturn.TotalAmount,

            Lines = purchaseReturn.Lines
                .Select(x => new PostingLineContext
                {
                    ProductId = x.ProductId,
                    WarehouseId = purchaseReturn.WarehouseId,
                    Quantity = x.Quantity,

                    // PurchaseReturnLine يستخدم UnitCost وليس UnitPrice
                    UnitPrice = x.UnitCost,

                    Description = x.Notes
                })
                .ToList()
        };

        await _postingEngine.PostDocumentAsync(
            context,
            cancellationToken);
    }

    public async Task ReversePurchaseReturnAsync(
        Guid purchaseReturnId,
        CancellationToken cancellationToken = default)
    {
        var purchaseReturn =
            await _purchaseReturnRepository.GetByIdWithLinesAsync(
                purchaseReturnId,
                cancellationToken);

        if (purchaseReturn is null)
            throw new InvalidOperationException("Purchase Return not found.");

        var context = new PostingContext
        {
            DocumentId = purchaseReturn.Id,
            DocumentType = DocumentType.PurchaseReturn,
            DocumentNumber = purchaseReturn.DocumentNumber,
            PostingDate = purchaseReturn.DocumentDate,
            FiscalYearId = purchaseReturn.FiscalYearId,
            CompanyId = purchaseReturn.CompanyId,
            BranchId = purchaseReturn.BranchId,
            ReferenceNumber = purchaseReturn.DocumentNumber,
            Description = purchaseReturn.Notes,
            Amount = purchaseReturn.TotalAmount,

            Lines = purchaseReturn.Lines
                .Select(x => new PostingLineContext
                {
                    ProductId = x.ProductId,
                    WarehouseId = purchaseReturn.WarehouseId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitCost,
                    Description = x.Notes
                })
                .ToList()
        };

        await _postingEngine.ReverseDocumentAsync(
            context,
            cancellationToken);
    }
}