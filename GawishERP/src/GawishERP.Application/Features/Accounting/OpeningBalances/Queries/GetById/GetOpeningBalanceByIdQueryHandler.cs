using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Queries.GetById;

public sealed class GetOpeningBalanceByIdQueryHandler
    : IRequestHandler<GetOpeningBalanceByIdQuery, Result<OpeningBalanceDetailsDto>>
{
    private readonly IJournalEntryRepository _journalEntryRepository;

    public GetOpeningBalanceByIdQueryHandler(
        IJournalEntryRepository journalEntryRepository)
    {
        _journalEntryRepository = journalEntryRepository;
    }

    public async Task<Result<OpeningBalanceDetailsDto>> Handle(
        GetOpeningBalanceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var journal =
            await _journalEntryRepository.GetByIdWithLinesAsync(
                request.Id,
                cancellationToken);

        if (journal is null)
        {
            return Result.Failure<OpeningBalanceDetailsDto>(
                new Error(
                    "OpeningBalance.NotFound",
                    "Opening balance was not found.",
                    ErrorType.NotFound));
        }

        if (journal.DocumentType != DocumentType.OpeningBalance)
        {
            return Result.Failure<OpeningBalanceDetailsDto>(
                new Error(
                    "OpeningBalance.Invalid",
                    "The requested document is not an opening balance.",
                    ErrorType.Validation));
        }

        var dto = new OpeningBalanceDetailsDto
        {
            Id = journal.Id,
            DocumentNumber = journal.DocumentNumber,
            DocumentDate = journal.DocumentDate,
            FiscalYearId = journal.FiscalYearId,
            CompanyId = journal.CompanyId,
            BranchId = journal.BranchId,
            ReferenceNumber = journal.ReferenceNumber,
            Notes = journal.Notes,
            Status = journal.Status,
            TotalDebit = journal.TotalDebit,
            TotalCredit = journal.TotalCredit,

            Lines = journal.Lines
                .Select(x => new OpeningBalanceLineDetailsDto
                {
                    AccountId = x.AccountId,
                    AccountCode = x.Account.Code,
                    AccountName = x.Account.Name,
                    Debit = x.Debit,
                    Credit = x.Credit,
                    Description = x.Description
                })
                .ToList()
        };

        return Result.Success(dto);
    }
}