using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Update;

public sealed record UpdateAccountCommand(
    Guid Id,
    string Name,
    Guid? ParentAccountId,
    bool IsPostingAccount)
    : IRequest<UpdateAccountResponse>;