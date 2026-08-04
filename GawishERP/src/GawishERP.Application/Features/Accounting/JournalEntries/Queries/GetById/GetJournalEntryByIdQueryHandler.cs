using GawishERP.Application.Features.Accounting.JournalEntries.DTOs;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Queries.GetById;

public sealed class GetJournalEntryByIdQueryHandler
    : IRequestHandler<GetJournalEntryByIdQuery, JournalEntryDto?>
{
    private readonly IJournalEntryRepository _repository;

    public GetJournalEntryByIdQueryHandler(
        IJournalEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<JournalEntryDto?> Handle(
        GetJournalEntryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity =
            await _repository.GetByIdForViewAsync(
                request.Id,
                cancellationToken);

        if (entity is null)
            return null;

        return JournalEntryDto.FromEntity(entity);
    }
}