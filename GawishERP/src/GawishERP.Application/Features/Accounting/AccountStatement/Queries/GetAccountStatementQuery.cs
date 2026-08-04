using GawishERP.Application.Features.Accounting.AccountStatement.Responses;
using MediatR;

namespace GawishERP.Application.Features.Accounting.AccountStatement.Queries;

public sealed record GetAccountStatementQuery(
    Guid AccountId,
    Guid FiscalYearId,
    DateTime? FromDate,
    DateTime? ToDate,
    Guid? CompanyId,
    Guid? BranchId)
    : IRequest<GetAccountStatementResponse>;