using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Finance.Payments.Commands.Create;

public sealed class CreateSupplierPaymentCommandHandler
    : IRequestHandler<CreateSupplierPaymentCommand, CreateSupplierPaymentResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly ILedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSupplierPaymentCommandHandler(
        ISupplierRepository supplierRepository,
        IAccountRepository accountRepository,
        IFiscalYearRepository fiscalYearRepository,
        IJournalEntryRepository journalEntryRepository,
        IDocumentNumberService documentNumberService,
        ILedgerPostingService ledgerPostingService,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _accountRepository = accountRepository;
        _fiscalYearRepository = fiscalYearRepository;
        _journalEntryRepository = journalEntryRepository;
        _documentNumberService = documentNumberService;
        _ledgerPostingService = ledgerPostingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateSupplierPaymentResponse> Handle(
        CreateSupplierPaymentCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            throw new InvalidOperationException("Payment amount must be greater than zero.");

        if (request.FiscalYearId == Guid.Empty || request.SupplierId == Guid.Empty || request.CashAccountId == Guid.Empty)
            throw new InvalidOperationException("Fiscal year, supplier and cash account are required.");

        var fiscalYear = await _fiscalYearRepository.GetByIdAsync(request.FiscalYearId);
        if (fiscalYear is null)
            throw new InvalidOperationException("Fiscal year not found.");
        if (!fiscalYear.IsOpen)
            throw new InvalidOperationException("Fiscal year is closed.");

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId);
        if (supplier is null)
            throw new InvalidOperationException("Supplier not found.");
        if (!supplier.IsActive)
            throw new InvalidOperationException("Supplier is inactive.");
        if (!supplier.AccountId.HasValue)
            throw new InvalidOperationException("Supplier has no accounting account assigned.");

        var cashAccount = await _accountRepository.GetByIdAsync(request.CashAccountId, cancellationToken);
        if (cashAccount is null)
            throw new InvalidOperationException("Cash/Bank account not found.");
        if (!cashAccount.IsActive || !cashAccount.IsPostingAccount)
            throw new InvalidOperationException("Cash/Bank account must be active and a posting account.");
        if (!cashAccount.IsCashAccount)
            throw new InvalidOperationException("Selected account is not configured as a cash/bank account.");

        var supplierAccount = await _accountRepository.GetByIdAsync(supplier.AccountId.Value, cancellationToken);
        if (supplierAccount is null || !supplierAccount.IsActive || !supplierAccount.IsPostingAccount)
            throw new InvalidOperationException("Supplier accounting account is invalid.");

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
            supplierAccount.Id,
            request.Amount,
            0m,
            "Supplier Payment - Supplier");

        journalEntry.AddLine(
            cashAccount.Id,
            0m,
            request.Amount,
            "Supplier Payment - Cash/Bank");

        journalEntry.Submit();
        journalEntry.Approve();
        journalEntry.Post();

        _journalEntryRepository.Add(journalEntry);

        await _ledgerPostingService.PostAsync(
            journalEntry,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateSupplierPaymentResponse
        {
            Id = journalEntry.Id,
            DocumentNumber = journalEntry.DocumentNumber,
            Status = journalEntry.Status.ToString(),
            Message = "Supplier payment posted successfully."
        };
    }
}
