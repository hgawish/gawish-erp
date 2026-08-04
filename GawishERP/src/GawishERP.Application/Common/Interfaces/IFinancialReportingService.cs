using GawishERP.Application.Features.Accounting.AccountStatement.Responses;
using GawishERP.Application.Features.Accounting.GeneralLedger.Responses;
using GawishERP.Application.Features.Accounting.TrialBalance.Responses;

namespace GawishERP.Application.Common.Interfaces;

public interface IFinancialReportingService
{
    Task<GetTrialBalanceResponse> GetTrialBalanceAsync(
        Guid fiscalYearId,
        Guid? companyId,
        Guid? branchId,
        CancellationToken cancellationToken = default);

    Task<GetGeneralLedgerResponse> GetGeneralLedgerAsync(
        Guid accountId,
        Guid fiscalYearId,
        DateTime? fromDate,
        DateTime? toDate,
        Guid? companyId,
        Guid? branchId,
        CancellationToken cancellationToken = default);

    Task<GetAccountStatementResponse> GetAccountStatementAsync(
        Guid accountId,
        Guid fiscalYearId,
        DateTime? fromDate,
        DateTime? toDate,
        Guid? companyId,
        Guid? branchId,
        CancellationToken cancellationToken = default);
}