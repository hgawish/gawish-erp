using GawishERP.Application.Features.Accounting.BalanceSheet.Responses;
using MediatR;

namespace GawishERP.Application.Features.Accounting.BalanceSheet.Queries;

public sealed record GetBalanceSheetQuery(
    Guid FiscalYearId,
    Guid? CompanyId,
    Guid? BranchId)
    : IRequest<GetBalanceSheetResponse>;