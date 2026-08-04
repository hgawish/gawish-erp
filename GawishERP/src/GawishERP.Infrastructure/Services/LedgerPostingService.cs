using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public sealed partial class LedgerPostingService : ILedgerPostingService
{
    private readonly ILedgerTransactionRepository _ledgerRepository;
    private readonly IAccountBalanceRepository _accountBalanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LedgerPostingService(
        ILedgerTransactionRepository ledgerRepository,
        IAccountBalanceRepository accountBalanceRepository,
        IUnitOfWork unitOfWork)
    {
        _ledgerRepository = ledgerRepository;
        _accountBalanceRepository = accountBalanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task PostAsync(
        JournalEntryHeader journalEntry,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(journalEntry);

        ValidateJournal(journalEntry);

        foreach (var line in journalEntry.Lines)
        {
            await PostLineAsync(
                journalEntry,
                line,
                cancellationToken);
        }

        // لا يوجد SaveChanges هنا
        // الـ Handler هو الذى يتحكم فى Transaction كاملة
    }
}