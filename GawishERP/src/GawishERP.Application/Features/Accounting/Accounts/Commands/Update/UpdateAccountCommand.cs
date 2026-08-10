using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Update;

public sealed record UpdateAccountCommand : IRequest<UpdateAccountResponse>
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool IsPostingAccount { get; init; }

    public Guid? ParentAccountId { get; init; }

    public Guid? FinancialStatementNodeId { get; init; }

    public bool IsCashAccount { get; init; }
}