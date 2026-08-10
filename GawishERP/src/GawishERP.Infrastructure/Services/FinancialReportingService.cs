using GawishERP.Application.Common.Interfaces;
using GawishERP.Infrastructure.Persistence;

namespace GawishERP.Infrastructure.Services;

public sealed partial class FinancialReportingService
    : IFinancialReportingService
{
    private readonly ApplicationDbContext _context;

    public FinancialReportingService(
        ApplicationDbContext context)
    {
        _context = context;
    }
}