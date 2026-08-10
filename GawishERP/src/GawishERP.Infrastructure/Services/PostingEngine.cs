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

    //=========================================================
    // Post Document
    //=========================================================

    public async Task<PostingResult> PostDocumentAsync(
        PostingContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        //=====================================================
        // Resolve Posting Accounts
        //=====================================================

        var resolutions =
            await _accountResolver.ResolveAsync(
                context,
                cancellationToken);

        if (resolutions is null || resolutions.Count == 0)
        {
            throw new InvalidOperationException(
                "No posting lines were resolved for this document.");
        }

        //=====================================================
        // Validate Balance
        //=====================================================

        var totalDebit =
            resolutions
                .Where(x => x.EntryType == PostingEntryType.Debit)
                .Sum(x => x.Amount);

        var totalCredit =
            resolutions
                .Where(x => x.EntryType == PostingEntryType.Credit)
                .Sum(x => x.Amount);

        if (totalDebit != totalCredit)
        {
            throw new InvalidOperationException(
                $"Journal entry is not balanced. " +
                $"Debit: {totalDebit}, " +
                $"Credit: {totalCredit}.");
        }

        if (totalDebit <= 0)
        {
            throw new InvalidOperationException(
                "Journal entry amount must be greater than zero.");
        }

        //=====================================================
        // Create Journal Header
        //=====================================================

        var journal =
            new JournalEntryHeader(
                context.DocumentNumber,
                context.PostingDate,
                context.FiscalYearId,
                context.DocumentType,
                context.ReferenceNumber
                    ?? context.DocumentNumber,
                context.Description,
                context.CompanyId,
                context.BranchId);

        //=====================================================
        // Add Journal Lines
        //=====================================================

        foreach (var item in resolutions)
        {
            if (item.AccountId == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "Posting line contains an empty account id.");
            }

            if (item.Amount <= 0)
            {
                throw new InvalidOperationException(
                    "Posting line amount must be greater than zero.");
            }

            switch (item.EntryType)
            {
                case PostingEntryType.Debit:

                    journal.AddLine(
                        item.AccountId,
                        item.Amount,
                        0m,
                        item.Description);

                    break;

                case PostingEntryType.Credit:

                    journal.AddLine(
                        item.AccountId,
                        0m,
                        item.Amount,
                        item.Description);

                    break;

                default:

                    throw new InvalidOperationException(
                        $"Unsupported posting entry type '{item.EntryType}'.");
            }
        }

        //=====================================================
        // Submit → Approve → Post
        //=====================================================

        journal.Submit();

        journal.Approve();

        journal.Post();

        //=====================================================
        // Persist
        //=====================================================

        await _context.JournalEntryHeaders.AddAsync(
            journal,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        //=====================================================
        // Result
        //=====================================================

        return new PostingResult
        {
            Success = true,

            JournalEntryId = journal.Id,

            Message =
                "Posting completed successfully.",

            Lines =
                journal.Lines
                    .Select(x => new PostingResultLine
                    {
                        AccountId = x.AccountId,

                        Debit = x.Debit,

                        Credit = x.Credit,

                        Description = x.Description
                    })
                    .ToList()
        };
    }

    //=========================================================
    // Reverse Document
    //=========================================================

    public async Task<PostingResult> ReverseDocumentAsync(
        PostingContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        //=====================================================
        // Find Original Journal
        //=====================================================

        var original =
            await _context.JournalEntryHeaders
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(
                    x =>
                        x.DocumentNumber ==
                        context.DocumentNumber,

                    cancellationToken);

        if (original is null)
        {
            throw new InvalidOperationException(
                $"Journal Entry '{context.DocumentNumber}' not found.");
        }

        //=====================================================
        // Validate Reverse State
        //=====================================================

        if (original.IsReversed)
        {
            throw new InvalidOperationException(
                "Journal entry has already been reversed.");
        }

        //=====================================================
        // Create Reverse Entry
        //=====================================================

        var reverseEntry =
            original.CreateReverseEntry(
                $"REV-{original.DocumentNumber}");

        //=====================================================
        // Post Reverse Entry
        //=====================================================

        reverseEntry.Submit();

        reverseEntry.Approve();

        reverseEntry.Post();

        //=====================================================
        // Mark Original As Reversed
        //=====================================================

        original.MarkAsReversed(
            reverseEntry.Id);

        //=====================================================
        // Persist
        //=====================================================

        await _context.JournalEntryHeaders.AddAsync(
            reverseEntry,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        //=====================================================
        // Result
        //=====================================================

        return new PostingResult
        {
            Success = true,

            JournalEntryId = reverseEntry.Id,

            Message =
                "Journal entry reversed successfully.",

            Lines =
                reverseEntry.Lines
                    .Select(x => new PostingResultLine
                    {
                        AccountId = x.AccountId,

                        Debit = x.Debit,

                        Credit = x.Credit,

                        Description = x.Description
                    })
                    .ToList()
        };
    }
}