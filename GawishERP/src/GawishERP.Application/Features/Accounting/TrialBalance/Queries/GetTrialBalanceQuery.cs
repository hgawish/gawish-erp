using GawishERP.Application.Features.Accounting.TrialBalance.Responses;
using MediatR;

namespace GawishERP.Application.Features.Accounting.TrialBalance.Queries;

public sealed record GetTrialBalanceQuery(
    Guid FiscalYearId,
    Guid? CompanyId,
    Guid? BranchId)
    : IRequest<GetTrialBalanceResponse>;