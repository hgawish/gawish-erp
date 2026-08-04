using GawishERP.Application.Features.Accounting.Accounts.Commands.Activate;
using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Deactivate;

public sealed record DeactivateAccountCommand(
    Guid Id)
    : IRequest<DeactivateAccountResponse>;