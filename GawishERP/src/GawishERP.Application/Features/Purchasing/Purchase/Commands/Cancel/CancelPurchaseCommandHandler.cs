using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Cancel;

public sealed class CancelPurchaseCommandHandler
    : IRequestHandler<CancelPurchaseCommand, CancelPurchaseResponse>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelPurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelPurchaseResponse> Handle(
        CancelPurchaseCommand request,
        CancellationToken cancellationToken)
    {
        var purchase =
            await _purchaseRepository.GetByIdWithLinesAsync(
                request.PurchaseId,
                cancellationToken);

        if (purchase is null)
            throw new InvalidOperationException(
                "Purchase document not found.");

        if (purchase.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Purchase document already cancelled.");

        if (purchase.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted purchase can be cancelled.");

        foreach (var line in purchase.Lines)
        {
            await _inventoryService.ReversePurchaseAsync(
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

        purchase.Cancel();

        _purchaseRepository.Update(purchase);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelPurchaseResponse
        {
            Id = purchase.Id,
            DocumentNumber = purchase.DocumentNumber,
            Status = purchase.Status.ToString(),
            Message = "Purchase cancelled successfully."
        };
    }
}