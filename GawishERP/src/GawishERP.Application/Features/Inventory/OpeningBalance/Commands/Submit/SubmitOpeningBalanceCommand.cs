using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Submit;

public sealed class SubmitOpeningBalanceCommand
    : IRequest
{
    public Guid Id { get; init; }
}