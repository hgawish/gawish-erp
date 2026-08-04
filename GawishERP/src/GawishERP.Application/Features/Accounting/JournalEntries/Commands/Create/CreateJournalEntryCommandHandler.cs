using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Accounting.JournalEntries.DTOs;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Create;

public sealed class CreateJournalEntryCommandHandler
    : IRequestHandler<CreateJournalEntryCommand, Result<Guid>>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateJournalEntryCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IAccountRepository accountRepository,
        IFiscalYearRepository fiscalYearRepository,
        IDocumentNumberService documentNumberService,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _accountRepository = accountRepository;
        _fiscalYearRepository = fiscalYearRepository;
        _documentNumberService = documentNumberService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateJournalEntryCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.JournalEntry;

        var fiscalYear =
            await _fiscalYearRepository.GetByIdAsync(dto.FiscalYearId);

        if (fiscalYear is null)
        {
            return Result.Failure<Guid>(
                new Error(
                    "FiscalYear.NotFound",
                    "Fiscal year not found.",
                    ErrorType.NotFound));
        }

        var documentNumber =
            await _documentNumberService.GenerateAsync(
                DocumentType.JournalEntry,
                cancellationToken);

        var journalEntry = new JournalEntryHeader(
            documentNumber,
            dto.JournalDate.ToDateTime(TimeOnly.MinValue),
            dto.FiscalYearId,
            DocumentType.JournalEntry,
            dto.ReferenceNumber,
            dto.Description,
            dto.CompanyId,
            dto.BranchId);

        foreach (var line in dto.Lines)
        {
            var account =
                await _accountRepository.GetByIdAsync(line.AccountId);

            if (account is null)
            {
                return Result.Failure<Guid>(
                    new Error(
                        "Account.NotFound",
                        $"Account '{line.AccountId}' not found.",
                        ErrorType.NotFound));
            }

            journalEntry.AddLine(
                line.AccountId,
                line.Debit,
                line.Credit,
                line.Description ?? string.Empty);
        }

        _journalEntryRepository.Add(journalEntry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(journalEntry.Id);
    }
}