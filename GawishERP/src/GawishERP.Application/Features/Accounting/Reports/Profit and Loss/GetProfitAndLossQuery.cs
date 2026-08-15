using GawishERP.Application.Features.Accounting.Reports.Profit_and_Loss.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Accounting.Reports.Profit_and_Loss;

public sealed record GetProfitAndLossQuery(
    Guid FiscalYearId,
    DateTime? FromDate,
    DateTime? ToDate,
    Guid? CompanyId,
    Guid? BranchId) : IRequest<GetProfitAndLossResponse>;
