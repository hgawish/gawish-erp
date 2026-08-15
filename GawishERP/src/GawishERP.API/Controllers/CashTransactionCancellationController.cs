using GawishERP.Application.Features.Accounting.JournalEntries.Commands.Cancel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api")]
public sealed class CashTransactionCancellationController : ControllerBase
{
    private readonly ISender _sender;

    public CashTransactionCancellationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("receipts/{id:guid}/cancel")]
    public async Task<IActionResult> CancelReceipt(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CancelJournalEntryCommand(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("payments/{id:guid}/cancel")]
    public async Task<IActionResult> CancelPayment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CancelJournalEntryCommand(id),
            cancellationToken);

        return Ok(result);
    }
}
