using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Approve;

public sealed record ApproveJournalEntryCommand(
    Guid JournalEntryId)
    : IRequest<Result>;