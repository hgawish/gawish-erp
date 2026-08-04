using GawishERP.Domain.Common;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Queries.GetList;

public sealed record GetJournalEntriesQuery(
    string? Search,
    DateTime? FromDate,
    DateTime? ToDate,
    DocumentStatus? Status,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<GetJournalEntriesResponse>;