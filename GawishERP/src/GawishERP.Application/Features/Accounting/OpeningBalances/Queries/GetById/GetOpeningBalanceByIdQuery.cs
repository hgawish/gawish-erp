using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Queries.GetById;

public sealed record GetOpeningBalanceByIdQuery(Guid Id)
    : IRequest<Result<OpeningBalanceDetailsDto>>;