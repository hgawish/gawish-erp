using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Post;

public sealed class PostPurchaseCommandHandler
    : IRequestHandler<PostPurchaseCommand, PostPurchaseResponse>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public PostPurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PostPurchaseResponse> Handle(
        PostPurchaseCommand request,
        CancellationToken cancellationToken)
    {
        var purchase =
            await _purchaseRepository.GetByIdWithLinesAsync(
                request.PurchaseId,
                cancellationToken);

        if (purchase is null)
            throw new InvalidOperationException(
                "Purchase document not found.");

        if (purchase.Status == DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Purchase document is already posted.");

        if (purchase.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Cancelled purchase cannot be posted.");

        // Change document status
        purchase.Post();

        // Update Inventory
        foreach (var line in purchase.Lines)
        {
            await _inventoryService.AddPurchaseAsync(
    line.ProductId,
    purchase.WarehouseId,
    line.Quantity,
    line.UnitCost,
    purchase.DocumentDate,
    purchase.Id,
    purchase.DocumentNumber,
    purchase.Notes,
    cancellationToken);
        }

        _purchaseRepository.Update(purchase);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PostPurchaseResponse
        {
            Id = purchase.Id,
            DocumentNumber = purchase.DocumentNumber,
            Status = purchase.Status.ToString(),
            Message = "Purchase posted successfully."
        };
    }
}