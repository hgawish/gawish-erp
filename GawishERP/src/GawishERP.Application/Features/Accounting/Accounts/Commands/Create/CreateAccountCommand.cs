using GawishERP.Domain.Common;
using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Create;

public sealed record CreateAccountCommand(
    string Code,
    string Name,
    Guid? ParentAccountId,
    AccountType AccountType,
    AccountNature Nature,
    bool IsPostingAccount)
    : IRequest<CreateAccountResponse>;