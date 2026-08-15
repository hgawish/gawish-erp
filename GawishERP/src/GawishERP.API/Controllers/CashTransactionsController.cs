using GawishERP.Application.Features.Finance.Payments.Commands.Create;
using GawishERP.Application.Features.Finance.Receipts.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api")]
public sealed class CashTransactionsController : ControllerBase
{
    private readonly ISender _sender;

    public CashTransactionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("receipts")]
    public async Task<IActionResult> CreateReceipt(
        [FromBody] CreateCustomerReceiptCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("payments")]
    public async Task<IActionResult> CreatePayment(
        [FromBody] CreateSupplierPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
}
