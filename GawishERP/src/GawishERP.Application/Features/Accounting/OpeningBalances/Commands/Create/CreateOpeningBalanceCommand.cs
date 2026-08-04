using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Accounting.OpeningBalances.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Create;

public sealed record CreateOpeningBalanceCommand(
    OpeningBalanceDto OpeningBalance)
    : IRequest<Result<Guid>>;