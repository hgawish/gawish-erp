using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Infrastructure.Services;

public sealed partial class LedgerPostingService
{
    private static void ValidateJournal(
        JournalEntryHeader journalEntry)
    {
        if (journalEntry.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Journal Entry must be posted before creating ledger transactions.");

        if (!journalEntry.Lines.Any())
            throw new InvalidOperationException(
                "Journal Entry contains no lines.");

        var totalDebit =
            journalEntry.Lines.Sum(x => x.Debit);

        var totalCredit =
            journalEntry.Lines.Sum(x => x.Credit);

        if (totalDebit != totalCredit)
            throw new InvalidOperationException(
                "Journal Entry is not balanced.");

        foreach (var line in journalEntry.Lines)
        {
            ValidateLine(line);
        }
    }

    private static void ValidateLine(
        JournalEntryLine line)
    {
        if (line.AccountId == Guid.Empty)
            throw new InvalidOperationException(
                "Account is required.");

        if (line.Debit < 0)
            throw new InvalidOperationException(
                "Debit cannot be negative.");

        if (line.Credit < 0)
            throw new InvalidOperationException(
                "Credit cannot be negative.");

        if (line.Debit == 0 && line.Credit == 0)
            throw new InvalidOperationException(
                "Both debit and credit cannot be zero.");

        if (line.Debit > 0 && line.Credit > 0)
            throw new InvalidOperationException(
                "Debit and Credit cannot both contain values.");
    }
}