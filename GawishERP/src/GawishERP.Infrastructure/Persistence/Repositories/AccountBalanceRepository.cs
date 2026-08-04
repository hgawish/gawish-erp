using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class AccountBalanceRepository
    : RepositoryBase<AccountBalance>, IAccountBalanceRepository
{
    public AccountBalanceRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<AccountBalance?> GetAsync(
        Guid accountId,
        Guid fiscalYearId,
        Guid? companyId = null,
        Guid? branchId = null)
    {
        return await Context.AccountBalances
            .FirstOrDefaultAsync(x =>
                x.AccountId == accountId &&
                x.FiscalYearId == fiscalYearId &&
                x.CompanyId == companyId &&
                x.BranchId == branchId);
    }

    public async Task<bool> ExistsAsync(
        Guid accountId,
        Guid fiscalYearId,
        Guid? companyId = null,
        Guid? branchId = null)
    {
        return await Context.AccountBalances
            .AnyAsync(x =>
                x.AccountId == accountId &&
                x.FiscalYearId == fiscalYearId &&
                x.CompanyId == companyId &&
                x.BranchId == branchId);
    }

    public async Task<List<AccountBalance>> GetAllAsync(
        Guid fiscalYearId,
        Guid? companyId = null,
        Guid? branchId = null)
    {
        return await Context.AccountBalances
            .Where(x =>
                x.FiscalYearId == fiscalYearId &&
                x.CompanyId == companyId &&
                x.BranchId == branchId)
            .OrderBy(x => x.Account.Code)
            .ToListAsync();
    }
    public async Task<List<AccountBalance>> GetTrialBalanceAsync(
    Guid fiscalYearId,
    Guid? companyId = null,
    Guid? branchId = null)
    {
        return await Context.AccountBalances
            .Include(x => x.Account)
            .Where(x =>
                x.FiscalYearId == fiscalYearId &&
                x.CompanyId == companyId &&
                x.BranchId == branchId)
            .OrderBy(x => x.Account.Code)
            .ToListAsync();
    }
    public void Add(AccountBalance balance)
    {
        Context.AccountBalances.Add(balance);
    }

    public void Update(AccountBalance balance)
    {
        Context.AccountBalances.Update(balance);
    }
}