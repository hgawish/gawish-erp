using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Create;

public sealed class CreatePurchaseCommandHandler
    : IRequestHandler<CreatePurchaseCommand, CreatePurchaseResponse>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        IDocumentNumberService documentNumberService,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _documentNumberService = documentNumberService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePurchaseResponse> Handle(
        CreatePurchaseCommand request,
        CancellationToken cancellationToken)
    {
        // Generate Document Number
        var documentNumber =
            await _documentNumberService.GenerateAsync(
                DocumentType.Purchase,
                cancellationToken);

        // Create Purchase Header
        var purchase = new PurchaseHeader(
            documentNumber,
            request.DocumentDate,
            request.InvoiceNumber,
            request.InvoiceDate,
            request.SupplierId,
            request.WarehouseId,
            request.Currency,
            request.ExchangeRate,
            request.Notes);

        // Add Lines
        foreach (var line in request.Lines)
        {
            purchase.AddLine(
                line.ProductId,
                line.Quantity,
                line.UnitCost,
                line.DiscountAmount,
                line.TaxAmount,
                line.BatchNumber,
                line.ExpiryDate,
                line.Notes);
        }

        // Save Draft Purchase
        _purchaseRepository.Add(purchase);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return Result
        return new CreatePurchaseResponse
        {
            Id = purchase.Id,
            DocumentNumber = purchase.DocumentNumber
        };
    }
}