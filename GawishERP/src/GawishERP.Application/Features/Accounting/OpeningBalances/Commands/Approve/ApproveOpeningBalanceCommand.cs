using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Approve;

public sealed record ApproveOpeningBalanceCommand(
    Guid Id)
    : IRequest<Result>;