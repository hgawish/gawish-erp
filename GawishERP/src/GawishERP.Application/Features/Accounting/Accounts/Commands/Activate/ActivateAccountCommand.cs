using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Activate;

public sealed record ActivateAccountCommand(
    Guid Id)
    : IRequest<ActivateAccountResponse>;