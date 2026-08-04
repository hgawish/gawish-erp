using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Queries.GetById;

public sealed record GetAccountByIdQuery(
    Guid Id)
    : IRequest<GetAccountByIdResponse>;