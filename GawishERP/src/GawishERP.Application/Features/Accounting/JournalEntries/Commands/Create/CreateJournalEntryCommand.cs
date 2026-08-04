using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Accounting.JournalEntries.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Create;

public sealed record CreateJournalEntryCommand(
    CreateJournalEntryDto JournalEntry)
    : IRequest<Result<Guid>>;