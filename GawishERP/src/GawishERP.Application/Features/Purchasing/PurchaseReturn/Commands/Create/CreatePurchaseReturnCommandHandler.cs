using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Create;

public sealed class CreatePurchaseReturnCommandHandler
    : IRequestHandler<CreatePurchaseReturnCommand, CreatePurchaseReturnResponse>
{
    private readonly IPurchaseReturnRepository _purchaseReturnRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseReturnCommandHandler(
        IPurchaseReturnRepository purchaseReturnRepository,
        IDocumentNumberService documentNumberService,
        IUnitOfWork unitOfWork)
    {
        _purchaseReturnRepository = purchaseReturnRepository;
        _documentNumberService = documentNumberService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePurchaseReturnResponse> Handle(
        CreatePurchaseReturnCommand request,
        CancellationToken cancellationToken)
    {
        //=========================================================
        // Generate ERP Document Number
        //=========================================================

        var documentNumber =
            await _documentNumberService.GenerateAsync(
                DocumentType.PurchaseReturn,
                cancellationToken);

        //=========================================================
        // Create Header
        //=========================================================

        var purchaseReturn = new PurchaseReturnHeader(
            documentNumber,
            request.DocumentDate,

            request.FiscalYearId,
            request.CompanyId,
            request.BranchId,

            request.PurchaseId,
            request.SupplierId,
            request.WarehouseId,
            request.ReturnReason,
            request.Notes);

        //=========================================================
        // Add Lines
        //=========================================================

        foreach (var line in request.Lines)
        {
            purchaseReturn.AddLine(
                line.PurchaseLineId,
                line.ProductId,
                line.Quantity,
                line.UnitCost,
                line.Notes);
        }

        //=========================================================
        // Save
        //=========================================================

        _purchaseReturnRepository.Add(purchaseReturn);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        //=========================================================
        // Response
        //=========================================================

        return new CreatePurchaseReturnResponse
        {
            Id = purchaseReturn.Id,
            DocumentNumber = purchaseReturn.DocumentNumber
        };
    }
}