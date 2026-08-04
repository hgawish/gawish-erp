using GawishERP.Application.Common.Results;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Queries.GetList;

public sealed class GetOpeningBalanceListQueryHandler
    : IRequestHandler<
        GetOpeningBalanceListQuery,
        Result<PagedResult<OpeningBalanceListDto>>>
{
    private readonly IJournalEntryRepository _journalEntryRepository;

    public GetOpeningBalanceListQueryHandler(
        IJournalEntryRepository journalEntryRepository)
    {
        _journalEntryRepository = journalEntryRepository;
    }

    public async Task<Result<PagedResult<OpeningBalanceListDto>>> Handle(
        GetOpeningBalanceListQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) =
            await _journalEntryRepository.GetOpeningBalancesAsync(
                request.Search,
                request.FromDate,
                request.ToDate,
                request.Status,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        var dtoItems =
            items.Select(x => new OpeningBalanceListDto
            {
                Id = x.Id,
                DocumentNumber = x.DocumentNumber,
                DocumentDate = x.DocumentDate,
                FiscalYearId = x.FiscalYearId,
                CompanyId = x.CompanyId,
                BranchId = x.BranchId,
                ReferenceNumber = x.ReferenceNumber,
                Notes = x.Notes,
                TotalDebit = x.TotalDebit,
                TotalCredit = x.TotalCredit,
                Status = x.Status
            }).ToList();

        var result =
            PagedResult<OpeningBalanceListDto>.Create(
                dtoItems,
                totalCount,
                request.PageNumber,
                request.PageSize);

        return Result.Success(result);
    }
}