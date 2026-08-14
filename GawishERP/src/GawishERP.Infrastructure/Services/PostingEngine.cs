using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Services;

public sealed class PostingEngine : IPostingEngine
{
    private readonly ApplicationDbContext _context;
    private readonly IAccountResolver _accountResolver;

    public PostingEngine(
        ApplicationDbContext context,
        IAccountResolver accountResolver)
    {
        _context = context;
        _accountResolver = accountResolver;
    }

    public async Task<PostingResult> PostDocumentAsync(
        PostingContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var resolutions = await _accountResolver.ResolveAsync(context, cancellationToken);

        if (resolutions is null || resolutions.Count == 0)
            throw new InvalidOperationException("No posting lines were resolved for this document.");

        var totalDebit = resolutions.Where(x => x.EntryType == PostingEntryType.Debit).Sum(x => x.Amount);
        var totalCredit = resolutions.Where(x => x.EntryType == PostingEntryType.Credit).Sum(x => x.Amount);

        if (totalDebit != totalCredit)
            throw new InvalidOperationException($"Journal entry is not balanced. Debit: {totalDebit}, Credit: {totalCredit}.");

        if (totalDebit <= 0)
            throw new InvalidOperationException("Journal entry amount must be greater than zero.");

        var journal = new JournalEntryHeader(
            context.DocumentNumber,
            context.PostingDate,
            context.FiscalYearId,
            context.DocumentType,
            context.ReferenceNumber ?? context.DocumentNumber,
            context.Description,
            context.CompanyId,
            context.BranchId);

        foreach (var item in resolutions)
        {
            if (item.AccountId == Guid.Empty)
                throw new InvalidOperationException("Posting line contains an empty account id.");

            if (item.Amount <= 0)
                throw new InvalidOperationException("Posting line amount must be greater than zero.");

            switch (item.EntryType)
            {
                case PostingEntryType.Debit:
                    journal.AddLine(item.AccountId, item.Amount, 0m, item.Description);
                    break;
                case PostingEntryType.Credit:
                    journal.AddLine(item.AccountId, 0m, item.Amount, item.Description);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported posting entry type '{item.EntryType}'.");
            }
        }

        journal.Submit();
        journal.Approve();
        journal.Post();

        // Intentionally do not call SaveChanges here.
        // The application handler owns the transaction boundary and commits
        // inventory, document and journal changes atomically.
        await _context.JournalEntryHeaders.AddAsync(journal, cancellationToken);

        return new PostingResult
        {
            Success = true,
            JournalEntryId = journal.Id,
            Message = "Posting completed successfully.",
            Lines = journal.Lines.Select(x => new PostingResultLine
            {
                AccountId = x.AccountId,
                Debit = x.Debit,
                Credit = x.Credit,
                Description = x.Description
            }).ToList()
        };
    }

    public async Task<PostingResult> ReverseDocumentAsync(
        PostingContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var original = await _context.JournalEntryHeaders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.DocumentNumber == context.DocumentNumber, cancellationToken);

        if (original is null)
            throw new InvalidOperationException($"Journal Entry '{context.DocumentNumber}' not found.");

        if (original.IsReversed)
            throw new InvalidOperationException("Journal entry has already been reversed.");

        var reverseEntry = original.CreateReverseEntry($"REV-{original.DocumentNumber}");
        reverseEntry.Submit();
        reverseEntry.Approve();
        reverseEntry.Post();
        original.MarkAsReversed(reverseEntry.Id);

        // Intentionally do not call SaveChanges here.
        await _context.JournalEntryHeaders.AddAsync(reverseEntry, cancellationToken);

        return new PostingResult
        {
            Success = true,
            JournalEntryId = reverseEntry.Id,
            Message = "Journal entry reversed successfully.",
            Lines = reverseEntry.Lines.Select(x => new PostingResultLine
            {
                AccountId = x.AccountId,
                Debit = x.Debit,
                Credit = x.Credit,
                Description = x.Description
            }).ToList()
        };
    }
}