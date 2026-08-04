using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Queries.GetById;

public sealed class GetOpeningBalanceByIdQueryHandler
    : IRequestHandler<GetOpeningBalanceByIdQuery, OpeningBalanceDetailsDto?>
{
    private readonly IOpeningBalanceRepository _openingBalanceRepository;

    public GetOpeningBalanceByIdQueryHandler(
        IOpeningBalanceRepository openingBalanceRepository)
    {
        _openingBalanceRepository = openingBalanceRepository;
    }

    public async Task<OpeningBalanceDetailsDto?> Handle(
        GetOpeningBalanceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var document =
            await _openingBalanceRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (document is null)
            return null;

        return new OpeningBalanceDetailsDto
        {
            Id = document.Id,
            DocumentNumber = document.DocumentNumber,
            WarehouseId = document.WarehouseId,
            DocumentDate = document.DocumentDate,
            Notes = document.Notes,

            // بديل IsPosted
            IsPosted = document.Status == DocumentStatus.Posted,

            Lines = document.Lines
                .Select(x => new OpeningBalanceLineDetailsDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    UnitCost = x.UnitCost,
                    TotalCost = x.TotalCost,
                    Notes = x.Notes
                })
                .ToList()
        };
    }
}