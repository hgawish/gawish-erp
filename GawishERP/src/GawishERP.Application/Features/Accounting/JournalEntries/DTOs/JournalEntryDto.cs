using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Application.Features.Accounting.JournalEntries.DTOs;

public sealed class JournalEntryDto
{
    public Guid Id { get; set; }

    public string DocumentNumber { get; set; } = string.Empty;

    public DateOnly JournalDate { get; set; }

    public string Description { get; set; } = string.Empty;

    public DocumentStatus Status { get; set; }

    public Guid FiscalYearId { get; set; }

    public List<JournalEntryLineDto> Lines { get; set; } = new();

    public static JournalEntryDto FromEntity(
        JournalEntryHeader entity)
    {
        return new JournalEntryDto
        {
            Id = entity.Id,

            DocumentNumber = entity.DocumentNumber,

            JournalDate = DateOnly.FromDateTime(entity.DocumentDate),

            Description = entity.Notes ?? string.Empty,

            FiscalYearId = entity.FiscalYearId,

            Status = entity.Status,

            Lines = entity.Lines
                .Select(line => new JournalEntryLineDto
                {
                    Id = line.Id,

                    AccountId = line.AccountId,

                    AccountCode = line.Account.Code,

                    AccountName = line.Account.Name,

                    Debit = line.Debit,

                    Credit = line.Credit,

                    Description = line.Description
                })
                .ToList()
        };
    }
}