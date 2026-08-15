using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Finance.Receipts.Commands.Create;

public sealed class CreateCustomerReceiptCommandHandler
    : IRequestHandler<CreateCustomerReceiptCommand, CreateCustomerReceiptResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly ILedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerReceiptCommandHandler(
        ICustomerRepository customerRepository,
        IAccountRepository accountRepository,
        IFiscalYearRepository fiscalYearRepository,
        IJournalEntryRepository journalEntryRepository,
        IDocumentNumberService documentNumberService,
        ILedgerPostingService ledgerPostingService,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _accountRepository = accountRepository;
        _fiscalYearRepository = fiscalYearRepository;
        _journalEntryRepository = journalEntryRepository;
        _documentNumberService = documentNumberService;
        _ledgerPostingService = ledgerPostingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCustomerReceiptResponse> Handle(
        CreateCustomerReceiptCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            throw new InvalidOperationException("Receipt amount must be greater than zero.");

        if (request.FiscalYearId == Guid.Empty || request.CustomerId == Guid.Empty || request.CashAccountId == Guid.Empty)
            throw new InvalidOperationException("Fiscal year, customer and cash account are required.");

        var fiscalYear = await _fiscalYearRepository.GetByIdAsync(request.FiscalYearId);
        if (fiscalYear is null)
            throw new InvalidOperationException("Fiscal year not found.");
        if (!fiscalYear.IsOpen)
            throw new InvalidOperationException("Fiscal year is closed.");

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer is null)
            throw new InvalidOperationException("Customer not found.");
        if (!customer.IsActive)
            throw new InvalidOperationException("Customer is inactive.");
        if (!customer.AccountId.HasValue)
            throw new InvalidOperationException("Customer has no accounting account assigned.");

        var cashAccount = await _accountRepository.GetByIdAsync(request.CashAccountId, cancellationToken);
        if (cashAccount is null)
            throw new InvalidOperationException("Cash/Bank account not found.");
        if (!cashAccount.IsActive || !cashAccount.IsPostingAccount)
            throw new InvalidOperationException("Cash/Bank account must be active and a posting account.");
        if (!cashAccount.IsCashAccount)
            throw new InvalidOperationException("Selected account is not configured as a cash/bank account.");

        var customerAccount = await _accountRepository.GetByIdAsync(customer.AccountId.Value, cancellationToken);
        if (customerAccount is null || !customerAccount.IsActive || !customerAccount.IsPostingAccount)
            throw new InvalidOperationException("Customer accounting account is invalid.");

        var documentNumber = await _documentNumberService.GenerateAsync(
            DocumentType.JournalEntry,
            cancellationToken);

        var journalEntry = new JournalEntryHeader(
            documentNumber,
            request.TransactionDate,
            request.FiscalYearId,
            DocumentType.JournalEntry,
            string.IsNullOrWhiteSpace(request.ReferenceNumber) ? documentNumber : request.ReferenceNumber,
            request.Notes,
            request.CompanyId,
            request.BranchId);

        journalEntry.AddLine(
            cashAccount.Id,
            request.Amount,
            0m,
            "Customer Receipt - Cash/Bank");

        journalEntry.AddLine(
            customerAccount.Id,
            0m,
            request.Amount,
            "Customer Receipt - Customer");

        journalEntry.Submit();
        journalEntry.Approve();
        journalEntry.Post();

        _journalEntryRepository.Add(journalEntry);

        await _ledgerPostingService.PostAsync(
            journalEntry,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCustomerReceiptResponse
        {
            Id = journalEntry.Id,
            DocumentNumber = journalEntry.DocumentNumber,
            Status = journalEntry.Status.ToString(),
            Message = "Customer receipt posted successfully."
        };
    }
}
