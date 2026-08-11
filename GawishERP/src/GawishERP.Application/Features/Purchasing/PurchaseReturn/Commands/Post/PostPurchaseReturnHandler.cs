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
    private readonly IPurchaseReturnPostingService _purchaseReturnPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public PostPurchaseReturnHandler(
        IPurchaseReturnRepository purchaseReturnRepository,
        IPurchaseRepository purchaseRepository,
        IPurchaseReturnPostingService purchaseReturnPostingService,
        IUnitOfWork unitOfWork)
    {
        _purchaseReturnRepository = purchaseReturnRepository;
        _purchaseRepository = purchaseRepository;
        _purchaseReturnPostingService = purchaseReturnPostingService;
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

        //=========================================================
        // Quantity Validation
        //=========================================================

        foreach (var returnLine in purchaseReturn.Lines)
        {
            var purchaseLine =
                purchase.Lines.FirstOrDefault(
                    x => x.Id == returnLine.PurchaseLineId);

            if (purchaseLine is null)
                throw new InvalidOperationException(
                    "Original Purchase Line not found.");

            var previouslyReturnedQuantity =
                await _purchaseReturnRepository.GetReturnedQuantityAsync(
                    returnLine.PurchaseLineId,
                    cancellationToken);

            var totalReturnedQuantity =
                previouslyReturnedQuantity + returnLine.Quantity;

            if (totalReturnedQuantity > purchaseLine.Quantity)
            {
                throw new InvalidOperationException(
                    $"Total returned quantity ({totalReturnedQuantity}) " +
                    $"exceeds purchased quantity ({purchaseLine.Quantity}) " +
                    $"for product {purchaseLine.ProductId}.");
            }
        }

        //=========================================================
        // Change document status
        //=========================================================

        purchaseReturn.Post();

        //=========================================================
        // Inventory + Accounting Posting
        //=========================================================

        await _purchaseReturnPostingService.PostPurchaseReturnAsync(
            purchaseReturn,
            cancellationToken);

        //=========================================================
        // Persist Purchase Return
        //=========================================================

        _purchaseReturnRepository.Update(purchaseReturn);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new PostPurchaseReturnResponse
        {
            Id = purchaseReturn.Id,
            DocumentNumber = purchaseReturn.DocumentNumber,
            Status = purchaseReturn.Status.ToString(),
            Message = "Purchase Return posted successfully."
        };
    }
}