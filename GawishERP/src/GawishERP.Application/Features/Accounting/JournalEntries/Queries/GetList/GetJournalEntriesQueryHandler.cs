using GawishERP.Application.Features.Accounting.JournalEntries.DTOs;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Queries.GetList;

public sealed class GetJournalEntriesQueryHandler
    : IRequestHandler<GetJournalEntriesQuery, GetJournalEntriesResponse>
{
    private readonly IJournalEntryRepository _repository;

    public GetJournalEntriesQueryHandler(
        IJournalEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetJournalEntriesResponse> Handle(
        GetJournalEntriesQuery request,
        CancellationToken cancellationToken)
    {
        var result =
            await _repository.GetAllAsync(
                request.Search,
                request.FromDate,
                request.ToDate,
                request.Status,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        return new GetJournalEntriesResponse
        {
            TotalCount = result.TotalCount,

            Items = result.Items
                .Select(JournalEntryDto.FromEntity)
                .ToList()
        };
    }
}