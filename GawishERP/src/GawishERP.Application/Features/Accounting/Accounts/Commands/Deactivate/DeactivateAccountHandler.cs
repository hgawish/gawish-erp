using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Deactivate;

public sealed class DeactivateAccountHandler
    : IRequestHandler<DeactivateAccountCommand, DeactivateAccountResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateAccountHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeactivateAccountResponse> Handle(
        DeactivateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (account is null)
            throw new InvalidOperationException("Account not found.");

        account.Deactivate();

        _accountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeactivateAccountResponse
        {
            Id = account.Id,
            Code = account.Code,
            Name = account.Name,
            Message = "Account deactivated successfully."
        };
    }
}