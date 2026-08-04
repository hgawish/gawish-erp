using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Cancel;

public sealed record CancelOpeningBalanceCommand(Guid Id)
    : IRequest<Result>;