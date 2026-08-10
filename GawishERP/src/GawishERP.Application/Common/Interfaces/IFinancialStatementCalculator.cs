using GawishERP.Application.Features.FinancialReporting.Dtos;
using GawishERP.Domain.Common;

namespace GawishERP.Application.Common.Interfaces;

public interface IFinancialStatementCalculator
{
    Task<List<FinancialStatementNodeDto>> CalculateAsync(
        FinancialStatementType statementType,
        DateTime asOfDate,
        CancellationToken cancellationToken = default);
}