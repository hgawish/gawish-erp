using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Cancel;

public sealed record CancelJournalEntryCommand(Guid JournalEntryId)
    : IRequest<Result>;