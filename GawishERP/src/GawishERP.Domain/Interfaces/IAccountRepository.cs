using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IAccountRepository
{
    void Add(Account account);

    void Update(Account account);

    Task<Account?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Account?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> HasChildrenAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task<(List<Account> Items, int TotalCount)> GetAllAsync(
        string? search,
        Guid? parentAccountId,
        bool? isPostingAccount,
        bool? isActive,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}