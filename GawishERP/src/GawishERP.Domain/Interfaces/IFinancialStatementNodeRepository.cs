using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IFinancialStatementNodeRepository
{
    void Add(FinancialStatementNode entity);

    void Update(FinancialStatementNode entity);

    void Remove(FinancialStatementNode entity);

    Task<FinancialStatementNode?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<FinancialStatementNode?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<List<FinancialStatementNode>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    IQueryable<FinancialStatementNode> GetQueryable();
}