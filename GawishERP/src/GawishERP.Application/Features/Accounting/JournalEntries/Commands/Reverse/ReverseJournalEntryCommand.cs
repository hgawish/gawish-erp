using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Reverse;

public sealed record ReverseJournalEntryCommand(
    Guid JournalEntryId)
    : IRequest<Result>;