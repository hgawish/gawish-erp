using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Post;

public sealed record PostOpeningBalanceCommand(
    Guid Id
) : IRequest<Result<Guid>>;