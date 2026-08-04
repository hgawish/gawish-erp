using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Post;

public sealed record PostJournalEntryCommand(Guid JournalEntryId)
    : IRequest<Result>;