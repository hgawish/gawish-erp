using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class PostingProfileRepository
    : IPostingProfileRepository
{
    private readonly ApplicationDbContext _context;

    public PostingProfileRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    //=========================================================
    // Get By Document Type
    //=========================================================

    public async Task<PostingProfile?> GetByDocumentTypeAsync(
        DocumentType documentType,
        CancellationToken cancellationToken = default)
    {
        return await _context.PostingProfiles
            .Include(x => x.DebitAccount)
            .Include(x => x.CreditAccount)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Account)
            .FirstOrDefaultAsync(
                x =>
                    x.DocumentType == documentType &&
                    x.IsActive,
                cancellationToken);
    }

    //=========================================================
    // Get By Code
    //=========================================================

    public async Task<PostingProfile?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        code = code.Trim();

        return await _context.PostingProfiles
            .Include(x => x.DebitAccount)
            .Include(x => x.CreditAccount)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Account)
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    //=========================================================
    // Get All
    //=========================================================

    public async Task<List<PostingProfile>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.PostingProfiles
            .Include(x => x.DebitAccount)
            .Include(x => x.CreditAccount)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Account)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    //=========================================================
    // Add
    //=========================================================

    public async Task AddAsync(
        PostingProfile postingProfile,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(postingProfile);

        await _context.PostingProfiles.AddAsync(
            postingProfile,
            cancellationToken);
    }

    //=========================================================
    // Update
    //=========================================================

    public Task UpdateAsync(
        PostingProfile postingProfile,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(postingProfile);

        _context.PostingProfiles.Update(
            postingProfile);

        return Task.CompletedTask;
    }

    //=========================================================
    // Delete
    //=========================================================

    public Task DeleteAsync(
        PostingProfile postingProfile,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(postingProfile);

        _context.PostingProfiles.Remove(
            postingProfile);

        return Task.CompletedTask;
    }
}