using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Post;

public sealed class PostOpeningBalanceCommand
    : IRequest
{
    public Guid Id { get; init; }
}