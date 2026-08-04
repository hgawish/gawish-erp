using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IAccountBalanceRepository
{
    Task<AccountBalance?> GetAsync(
        Guid accountId,
        Guid fiscalYearId,
        Guid? companyId = null,
        Guid? branchId = null);

    Task<bool> ExistsAsync(
        Guid accountId,
        Guid fiscalYearId,
        Guid? companyId = null,
        Guid? branchId = null);

    Task<List<AccountBalance>> GetAllAsync(
        Guid fiscalYearId,
        Guid? companyId = null,
        Guid? branchId = null);

    void Add(AccountBalance balance);

    void Update(AccountBalance balance);
    Task<List<AccountBalance>> GetTrialBalanceAsync(
    Guid fiscalYearId,
    Guid? companyId = null,
    Guid? branchId = null);
}