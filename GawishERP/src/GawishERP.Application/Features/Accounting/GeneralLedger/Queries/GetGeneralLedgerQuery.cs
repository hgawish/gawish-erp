using GawishERP.Application.Features.Accounting.GeneralLedger.Responses;
using MediatR;

namespace GawishERP.Application.Features.Accounting.GeneralLedger.Queries;

public sealed record GetGeneralLedgerQuery(
    Guid AccountId,
    Guid FiscalYearId,
    DateTime? FromDate,
    DateTime? ToDate,
    Guid? CompanyId,
    Guid? BranchId)
    : IRequest<GetGeneralLedgerResponse>;