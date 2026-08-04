using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Create;

public sealed class CreateAccountHandler
    : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateAccountResponse> Handle(
        CreateAccountCommand request,
        CancellationToken cancellationToken)
    {
        // ==========================================
        // Duplicate Code Validation
        // ==========================================

        if (await _accountRepository.ExistsByCodeAsync(
                request.Code,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Account code '{request.Code}' already exists.");
        }

        // ==========================================
        // Parent Validation
        // ==========================================

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

        // ==========================================
        // Create Entity
        // ==========================================

        var account = new Account(
            request.Code,
            request.Name,
            request.AccountType,
            request.Nature,
            request.IsPostingAccount,
            request.ParentAccountId);

        _accountRepository.Add(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAccountResponse
        {
            Id = account.Id,
            Code = account.Code,
            Name = account.Name,
            Message = "Account created successfully."
        };
    }
}