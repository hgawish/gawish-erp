using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Account account)
    {
        _context.Accounts.Add(account);
    }

    public void Update(Account account)
    {
        _context.Accounts.Update(account);
    }

    public async Task<Account?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(x => x.ParentAccount)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Account?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(x => x.ParentAccount)
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AnyAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task<bool> HasChildrenAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AnyAsync(
                x => x.ParentAccountId == accountId,
                cancellationToken);
    }

    public async Task<(List<Account> Items, int TotalCount)> GetAllAsync(
        string? search,
        Guid? parentAccountId,
        bool? isPostingAccount,
        bool? isActive,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Accounts
            .Include(x => x.ParentAccount)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Code.Contains(search) ||
                x.Name.Contains(search));
        }

        if (parentAccountId.HasValue)
        {
            query = query.Where(x =>
                x.ParentAccountId == parentAccountId.Value);
        }

        if (isPostingAccount.HasValue)
        {
            query = query.Where(x =>
                x.IsPostingAccount == isPostingAccount.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(x =>
                x.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Code)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}