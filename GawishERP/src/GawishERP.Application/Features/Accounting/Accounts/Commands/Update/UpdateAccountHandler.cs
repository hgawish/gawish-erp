using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Update;

public sealed class UpdateAccountHandler
    : IRequestHandler<UpdateAccountCommand, UpdateAccountResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateAccountResponse> Handle(
        UpdateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var account =
            await _accountRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (account is null)
            throw new InvalidOperationException(
                "Account not found.");

        if (request.ParentAccountId == request.Id)
            throw new InvalidOperationException(
                "Account cannot be parent of itself.");

        if (request.ParentAccountId.HasValue)
        {
            var parent =
                await _accountRepository.GetByIdAsync(
                    request.ParentAccountId.Value,
                    cancellationToken);

            if (parent is null)
                throw new InvalidOperationException(
                    "Parent account not found.");

            if (parent.IsPostingAccount)
                throw new InvalidOperationException(
                    "Posting account cannot be selected as parent account.");
        }

        if (request.IsPostingAccount &&
            await _accountRepository.HasChildrenAsync(
                account.Id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Account has child accounts and cannot be converted to posting account.");
        }

        account.Update(
            request.Name,
            request.IsPostingAccount,
            request.ParentAccountId);

        _accountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateAccountResponse
        {
            Id = account.Id,
            Code = account.Code,
            Name = account.Name,
            Message = "Account updated successfully."
        };
    }
}