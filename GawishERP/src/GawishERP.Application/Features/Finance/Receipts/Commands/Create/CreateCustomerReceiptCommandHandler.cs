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
    private readonly ISalesRepository _salesRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly ILedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerReceiptCommandHandler(
        ICustomerRepository customerRepository,
        ISalesRepository salesRepository,
        IAccountRepository accountRepository,
        IFiscalYearRepository fiscalYearRepository,
        IJournalEntryRepository journalEntryRepository,
        IDocumentNumberService documentNumberService,
        ILedgerPostingService ledgerPostingService,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _salesRepository = salesRepository;
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

        string referenceNumber = request.ReferenceNumber;

        if (request.SalesId.HasValue)
        {
            if (request.SalesId.Value == Guid.Empty)
                throw new InvalidOperationException("Sales Id is invalid.");

            var sales = await _salesRepository.GetByIdAsync(request.SalesId.Value, cancellationToken);

            if (sales is null)
                throw new InvalidOperationException("Sales invoice not found.");

            if (sales.Status != DocumentStatus.Posted)
                throw new InvalidOperationException("Only posted sales invoices can be settled.");

            if (sales.CustomerId != request.CustomerId)
                throw new InvalidOperationException("Sales invoice belongs to a different customer.");

            var settledAmount = _journalEntryRepository
                .GetQueryable()
                .Where(j =>
                    j.Status == DocumentStatus.Posted &&
                    !j.IsReversed &&
                    j.ReferenceNumber == sales.DocumentNumber)
                .SelectMany(j => j.Lines)
                .Where(l =>
                    l.AccountId == customerAccount.Id &&
                    l.Credit > 0 &&
                    l.Description == "Customer Receipt - Customer")
                .Select(l => l.Credit)
                .ToList()
                .Sum();

            var outstanding = sales.NetTotal - settledAmount;

            if (outstanding <= 0)
                throw new InvalidOperationException("Sales invoice is already fully settled.");

            if (request.Amount > outstanding)
            {
                throw new InvalidOperationException(
                    $"Receipt amount ({request.Amount:0.00}) exceeds outstanding amount ({outstanding:0.00}) for sales invoice {sales.DocumentNumber}.");
            }

            // A receipt linked to a sales invoice uses the invoice document number
            // as its accounting reference so the settlement remains traceable.
            referenceNumber = sales.DocumentNumber;
        }

        var documentNumber = await _documentNumberService.GenerateAsync(
            DocumentType.JournalEntry,
            cancellationToken);

        var journalEntry = new JournalEntryHeader(
            documentNumber,
            request.TransactionDate,
            request.FiscalYearId,
            DocumentType.JournalEntry,
            string.IsNullOrWhiteSpace(referenceNumber) ? documentNumber : referenceNumber,
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
            Message = request.SalesId.HasValue
                ? "Customer receipt posted and linked to sales invoice successfully."
                : "Customer receipt posted successfully."
        };
    }
}
