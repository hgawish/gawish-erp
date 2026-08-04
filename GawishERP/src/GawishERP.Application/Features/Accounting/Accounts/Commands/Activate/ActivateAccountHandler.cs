using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Activate;

public sealed class ActivateAccountHandler
    : IRequestHandler<ActivateAccountCommand, ActivateAccountResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateAccountHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActivateAccountResponse> Handle(
        ActivateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (account is null)
            throw new InvalidOperationException("Account not found.");

        account.Activate();

        _accountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ActivateAccountResponse
        {
            Id = account.Id,
            Code = account.Code,
            Name = account.Name,
            Message = "Account activated successfully."
        };
    }
}