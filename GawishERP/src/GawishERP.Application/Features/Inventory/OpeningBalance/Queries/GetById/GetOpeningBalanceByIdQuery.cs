using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Queries.GetById;

public sealed class GetOpeningBalanceByIdQuery
    : IRequest<OpeningBalanceDetailsDto?>
{
    public Guid Id { get; init; }
}