using GawishERP.Application.Features.Accounting.TrialBalance.DTOs;
using GawishERP.Application.Features.FinancialReporting.Dtos;

namespace GawishERP.Application.Common.Interfaces;

public interface IFinancialReportingService
{
    //==========================================================
    // Financial Statements
    //==========================================================

    Task<BalanceSheetDto> GetBalanceSheetAsync(
        DateTime asOfDate,
        CancellationToken cancellationToken = default);

    Task<IncomeStatementDto> GetIncomeStatementAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<CashFlowDto> GetCashFlowAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    //==========================================================
    // Accounting Reports
    //==========================================================

    Task<TrialBalanceReportDto> GetTrialBalanceAsync(
    DateTime asOfDate,
    CancellationToken cancellationToken = default);

    Task<GeneralLedgerDto> GetGeneralLedgerAsync(
        Guid accountId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<AccountStatementDto> GetAccountStatementAsync(
        Guid accountId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}