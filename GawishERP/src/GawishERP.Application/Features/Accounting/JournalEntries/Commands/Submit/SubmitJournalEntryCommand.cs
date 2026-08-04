using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Submit;

public sealed record SubmitJournalEntryCommand(
    Guid JournalEntryId)
    : IRequest<Result>;