using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Submit;

public sealed record SubmitOpeningBalanceCommand(
    Guid Id)
    : IRequest<Result>;