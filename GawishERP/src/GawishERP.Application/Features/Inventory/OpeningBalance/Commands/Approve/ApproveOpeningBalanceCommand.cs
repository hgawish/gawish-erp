using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Approve;

public sealed class ApproveOpeningBalanceCommand
    : IRequest
{
    public Guid Id { get; init; }
}