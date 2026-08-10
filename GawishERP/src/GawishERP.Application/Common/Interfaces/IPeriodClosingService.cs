namespace GawishERP.Application.Common.Interfaces;

public interface IPeriodClosingService
{
    Task CloseMonthAsync(
        Guid fiscalYearId,
        int month,
        CancellationToken cancellationToken = default);
}