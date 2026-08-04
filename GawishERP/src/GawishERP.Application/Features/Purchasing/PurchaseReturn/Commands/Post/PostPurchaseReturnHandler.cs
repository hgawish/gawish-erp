using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Post;

public sealed class PostPurchaseReturnHandler
    : IRequestHandler<PostPurchaseReturnCommand, PostPurchaseReturnResponse>
{
    private readonly IPurchaseReturnRepository _purchaseReturnRepository;
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public PostPurchaseReturnHandler(
        IPurchaseReturnRepository purchaseReturnRepository,
        IPurchaseRepository purchaseRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _purchaseReturnRepository = purchaseReturnRepository;
        _purchaseRepository = purchaseRepository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PostPurchaseReturnResponse> Handle(
        PostPurchaseReturnCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseReturn =
            await _purchaseReturnRepository.GetByIdWithLinesAsync(
                request.PurchaseReturnId,
                cancellationToken);

        if (purchaseReturn is null)
            throw new InvalidOperationException(
                "Purchase Return not found.");

        if (purchaseReturn.Status == DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Purchase Return already posted.");

        if (purchaseReturn.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Cancelled Purchase Return cannot be posted.");

        var purchase =
            await _purchaseRepository.GetByIdWithLinesAsync(
                purchaseReturn.PurchaseId,
                cancellationToken);

        if (purchase is null)
            throw new InvalidOperationException(
                "Original Purchase not found.");

        if (purchase.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Original Purchase must be posted.");

        // ===========================================
        // Quantity Validation
        // ===========================================

        foreach (var returnLine in purchaseReturn.Lines)
        {
            var purchaseLine =
                purchase.Lines.FirstOrDefault(
                    x => x.Id == returnLine.PurchaseLineId);

            if (purchaseLine is null)
                throw new InvalidOperationException(
                    "Original Purchase Line not found.");

            if (returnLine.Quantity > purchaseLine.Quantity)
                throw new InvalidOperationException(
                    $"Returned quantity exceeds purchased quantity for product {purchaseLine.ProductId}.");
        }

        // ===========================================
        // Inventory
        // ===========================================

        purchaseReturn.Post();

        foreach (var line in purchaseReturn.Lines)
        {
            await _inventoryService.AddPurchaseReturnAsync(
                line.ProductId,
                purchaseReturn.WarehouseId,
                line.Quantity,
                line.UnitCost,
                purchaseReturn.DocumentDate,
                purchaseReturn.Id,
                purchaseReturn.DocumentNumber,
                purchaseReturn.Notes,
                cancellationToken);
        }

        _purchaseReturnRepository.Update(purchaseReturn);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PostPurchaseReturnResponse
        {
            Id = purchaseReturn.Id,
            DocumentNumber = purchaseReturn.DocumentNumber,
            Status = purchaseReturn.Status.ToString(),
            Message = "Purchase Return posted successfully."
        };
    }
}