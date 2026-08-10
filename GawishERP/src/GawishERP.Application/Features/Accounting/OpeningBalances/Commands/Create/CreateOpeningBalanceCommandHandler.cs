using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Accounting.OpeningBalances.DTOs;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Create;

public sealed class CreateOpeningBalanceCommandHandler
    : IRequestHandler<CreateOpeningBalanceCommand, Result<Guid>>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOpeningBalanceCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IDocumentNumberService documentNumberService,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _documentNumberService = documentNumberService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateOpeningBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.OpeningBalance;

        //=========================================================
        // Validate Opening Balance uniqueness
        // One Opening Balance per:
        // Fiscal Year + Company + Branch
        //=========================================================

        var exists =
            await _journalEntryRepository.ExistsOpeningBalanceAsync(
                dto.FiscalYearId,
                dto.CompanyId,
                dto.BranchId,
                cancellationToken);

        if (exists)
        {
            return Result.Failure<Guid>(
                new Error(
                    "OpeningBalance.AlreadyExists",
                    "An Opening Balance already exists for this Fiscal Year, Company and Branch.",
                    ErrorType.Conflict));
        }

        //=========================================================
        // Generate Document Number
        //=========================================================

        var documentNumber =
            await _documentNumberService.GenerateAsync(
                DocumentType.OpeningBalance,
                dto.CompanyId,
                dto.BranchId,
                dto.FiscalYearId,
                cancellationToken);

        //=========================================================
        // Create Journal Entry
        //=========================================================

        var journalEntry = new JournalEntryHeader(
            documentNumber,
            dto.DocumentDate,
            dto.FiscalYearId,
            DocumentType.OpeningBalance,
            dto.ReferenceNumber,
            dto.Notes,
            dto.CompanyId,
            dto.BranchId);

        //=========================================================
        // Add Lines
        //=========================================================

        foreach (var line in dto.Lines)
        {
            journalEntry.AddLine(
                line.AccountId,
                line.Debit,
                line.Credit,
                line.Description);
        }

        //=========================================================
        // Save
        //=========================================================

        _journalEntryRepository.Add(journalEntry);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success(journalEntry.Id);
    }
}