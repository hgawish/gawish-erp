using GawishERP.Application.Features.Accounting.GeneralLedger.DTOs;
using GawishERP.Application.Features.Accounting.GeneralLedger.Responses;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.GeneralLedger.Queries;

public sealed class GetGeneralLedgerQueryHandler
    : IRequestHandler<GetGeneralLedgerQuery, GetGeneralLedgerResponse>
{
    private readonly ILedgerTransactionRepository _ledgerRepository;

    public GetGeneralLedgerQueryHandler(
        ILedgerTransactionRepository ledgerRepository)
    {
        _ledgerRepository = ledgerRepository;
    }

    public async Task<GetGeneralLedgerResponse> Handle(
        GetGeneralLedgerQuery request,
        CancellationToken cancellationToken)
    {
        var openingBalance =
            await _ledgerRepository.GetOpeningBalanceAsync(
                request.AccountId,
                request.FiscalYearId,
                request.FromDate,
                request.CompanyId,
                request.BranchId,
                cancellationToken);

        var transactions =
            await _ledgerRepository.GetAccountLedgerAsync(
                request.AccountId,
                request.FiscalYearId,
                request.FromDate,
                request.ToDate,
                request.CompanyId,
                request.BranchId,
                cancellationToken);

        decimal runningBalance = openingBalance;

        var rows = new List<GeneralLedgerRowDto>();

        foreach (var trx in transactions)
        {
            runningBalance += trx.Debit;
            runningBalance -= trx.Credit;

            rows.Add(new GeneralLedgerRowDto
            {
                PostingDate = trx.PostingDate,

                DocumentNumber = trx.DocumentNumber,

                DocumentType = trx.DocumentType.ToString(),

                Description = trx.Description,

                Debit = trx.Debit,

                Credit = trx.Credit,

                RunningBalance = runningBalance
            });
        }

        return new GetGeneralLedgerResponse
        {
            AccountId = request.AccountId,

            AccountCode = transactions.FirstOrDefault()?.Account?.Code ?? string.Empty,

            AccountName = transactions.FirstOrDefault()?.Account?.Name ?? string.Empty,

            OpeningBalance = openingBalance,

            Transactions = rows,

            TotalDebit = rows.Sum(x => x.Debit),

            TotalCredit = rows.Sum(x => x.Credit),

            ClosingBalance = runningBalance
        };
    }
}