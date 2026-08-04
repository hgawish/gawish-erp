using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.Inventory.OpeningBalance.Commands.CreateOpeningBalanceDocument;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Create;

public sealed class CreateOpeningBalanceDocumentCommandHandler
    : IRequestHandler<CreateOpeningBalanceDocumentCommand, Guid>
{
    private readonly IOpeningBalanceRepository _openingBalanceRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOpeningBalanceDocumentCommandHandler(
        IOpeningBalanceRepository openingBalanceRepository,
        IDocumentNumberService documentNumberService,
        IUnitOfWork unitOfWork)
    {
        _openingBalanceRepository = openingBalanceRepository;
        _documentNumberService = documentNumberService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateOpeningBalanceDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var documentNumber =
            await _documentNumberService.GenerateAsync(
                DocumentType.OpeningBalance,
                cancellationToken);

        var header = new OpeningBalanceHeader(
            documentNumber,
            request.WarehouseId,
            request.DocumentDate,
            request.Notes);

        foreach (var line in request.Lines)
        {
            header.AddLine(
                line.ProductId,
                line.Quantity,
                line.UnitCost,
                line.Notes);
        }

        _openingBalanceRepository.Add(header);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return header.Id;
    }
}