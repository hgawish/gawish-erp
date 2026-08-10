using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Infrastructure.Services;

public sealed partial class LedgerPostingService
{
    private async Task PostLineAsync(
        JournalEntryHeader header,
        JournalEntryLine line,
        CancellationToken cancellationToken)
    {
        var balance =
            await _accountBalanceRepository.GetAsync(
                line.AccountId,
                header.FiscalYearId,
                header.CompanyId,
                header.BranchId);

        if (balance is null)
        {
            balance = new AccountBalance(
                line.AccountId,
                header.FiscalYearId,
                header.CompanyId,
                header.BranchId);

            _accountBalanceRepository.Add(balance);
        }

        if (header.DocumentType == DocumentType.OpeningBalance)
        {
            balance.SetOpeningBalance(
                line.Debit,
                line.Credit);
        }
        else
        {
            balance.ApplyTransaction(
                line.Debit,
                line.Credit);
        }

        _accountBalanceRepository.Update(balance);

        var ledgerTransaction =
            new LedgerTransaction(
                journalEntryHeaderId: header.Id,
                journalEntryLineId: line.Id,
                accountId: line.AccountId,
                fiscalYearId: header.FiscalYearId,
                companyId: header.CompanyId,
                branchId: header.BranchId,
                postingDate: header.DocumentDate,
                documentNumber: header.DocumentNumber,
                documentType: header.DocumentType,
                debit: line.Debit,
                credit: line.Credit,
                runningBalance: balance.ClosingBalance,
                description: line.Description ?? header.Notes ?? string.Empty);

        _ledgerRepository.Add(ledgerTransaction);
    }
}