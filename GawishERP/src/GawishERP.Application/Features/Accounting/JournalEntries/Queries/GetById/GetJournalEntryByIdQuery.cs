using GawishERP.Application.Features.Accounting.JournalEntries.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Queries.GetById;

public sealed record GetJournalEntryByIdQuery(
    Guid Id)
    : IRequest<JournalEntryDto?>;