using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Queries.GetList;

public sealed class GetOpeningBalanceListQueryHandler
    : IRequestHandler<
        GetOpeningBalanceListQuery,
        PagedResult<OpeningBalanceListItemDto>>
{
    private readonly IOpeningBalanceRepository _openingBalanceRepository;

    public GetOpeningBalanceListQueryHandler(
        IOpeningBalanceRepository openingBalanceRepository)
    {
        _openingBalanceRepository = openingBalanceRepository;
    }

    public async Task<PagedResult<OpeningBalanceListItemDto>> Handle(
        GetOpeningBalanceListQuery request,
        CancellationToken cancellationToken)
    {
        var documents = await _openingBalanceRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.SortBy,
            request.Descending);

        var totalCount = await _openingBalanceRepository.CountAsync(
            request.Search);

        var items = documents
            .Select(document => new OpeningBalanceListItemDto
            {
                Id = document.Id,
                DocumentNumber = document.DocumentNumber,
                DocumentDate = document.DocumentDate,
                WarehouseId = document.WarehouseId,

                // التعديل هنا
                IsPosted = document.Status == DocumentStatus.Posted,

                LineCount = document.Lines.Count,
                TotalCost = document.Lines.Sum(x => x.TotalCost)
            })
            .ToList();

        return PagedResult<OpeningBalanceListItemDto>.Create(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}