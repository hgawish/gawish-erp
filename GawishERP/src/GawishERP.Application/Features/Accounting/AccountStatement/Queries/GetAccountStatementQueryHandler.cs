using GawishERP.Application.Features.Accounting.AccountStatement.DTOs;
using GawishERP.Application.Features.Accounting.AccountStatement.Responses;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.AccountStatement.Queries;

public sealed class GetAccountStatementQueryHandler
    : IRequestHandler<GetAccountStatementQuery, GetAccountStatementResponse>
{
    private readonly ILedgerTransactionRepository _ledgerRepository;

    public GetAccountStatementQueryHandler(
        ILedgerTransactionRepository ledgerRepository)
    {
        _ledgerRepository = ledgerRepository;
    }

    public async Task<GetAccountStatementResponse> Handle(
        GetAccountStatementQuery request,
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

        var rows = new List<AccountStatementRowDto>();

        foreach (var trx in transactions)
        {
            runningBalance += trx.Debit;
            runningBalance -= trx.Credit;

            rows.Add(new AccountStatementRowDto
            {
                PostingDate = trx.PostingDate,

                DocumentNumber = trx.DocumentNumber,

                DocumentType = trx.DocumentType.ToString(),

                Description = trx.Description,

                Debit = trx.Debit,

                Credit = trx.Credit,

                Balance = runningBalance
            });
        }

        return new GetAccountStatementResponse
        {
            AccountId = request.AccountId,

            AccountCode =
                transactions.FirstOrDefault()?.Account?.Code
                ?? string.Empty,

            AccountName =
                transactions.FirstOrDefault()?.Account?.Name
                ?? string.Empty,

            OpeningBalance = openingBalance,

            Transactions = rows,

            TotalDebit =
                rows.Sum(x => x.Debit),

            TotalCredit =
                rows.Sum(x => x.Credit),

            ClosingBalance = runningBalance
        };
    }
}