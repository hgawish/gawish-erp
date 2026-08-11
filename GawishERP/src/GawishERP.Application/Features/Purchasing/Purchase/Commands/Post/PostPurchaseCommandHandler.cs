using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Post;

public sealed class PostPurchaseCommandHandler
    : IRequestHandler<PostPurchaseCommand, PostPurchaseResponse>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IPurchasePostingService _purchasePostingService;
    private readonly IUnitOfWork _unitOfWork;

    public PostPurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        IPurchasePostingService purchasePostingService,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _purchasePostingService = purchasePostingService;
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

        //=========================================================
        // Change document status
        //=========================================================

        purchase.Post();

        //=========================================================
        // Inventory + Accounting Posting
        //=========================================================

        await _purchasePostingService.PostPurchaseInvoiceAsync(
            purchase,
            cancellationToken);

        //=========================================================
        // Persist Purchase
        //=========================================================

        _purchaseRepository.Update(purchase);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new PostPurchaseResponse
        {
            Id = purchase.Id,
            DocumentNumber = purchase.DocumentNumber,
            Status = purchase.Status.ToString(),
            Message = "Purchase posted successfully."
        };
    }
}