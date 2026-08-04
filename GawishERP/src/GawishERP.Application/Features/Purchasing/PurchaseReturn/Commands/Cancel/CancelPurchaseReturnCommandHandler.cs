using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Cancel;

public sealed class CancelPurchaseReturnCommandHandler
    : IRequestHandler<CancelPurchaseReturnCommand, CancelPurchaseReturnResponse>
{
    private readonly IPurchaseReturnRepository _purchaseReturnRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelPurchaseReturnCommandHandler(
        IPurchaseReturnRepository purchaseReturnRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _purchaseReturnRepository = purchaseReturnRepository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelPurchaseReturnResponse> Handle(
        CancelPurchaseReturnCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseReturn =
            await _purchaseReturnRepository.GetByIdWithLinesAsync(
                request.PurchaseReturnId,
                cancellationToken);

        if (purchaseReturn is null)
            throw new InvalidOperationException(
                "Purchase Return document not found.");

        if (purchaseReturn.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Purchase Return already cancelled.");

        if (purchaseReturn.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted Purchase Return can be cancelled.");

        foreach (var line in purchaseReturn.Lines)
        {
            await _inventoryService.AddPurchaseAsync(
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

        purchaseReturn.Cancel();

        _purchaseReturnRepository.Update(purchaseReturn);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelPurchaseReturnResponse
        {
            Id = purchaseReturn.Id,
            DocumentNumber = purchaseReturn.DocumentNumber,
            Status = purchaseReturn.Status.ToString(),
            Message = "Purchase Return cancelled successfully."
        };
    }
}