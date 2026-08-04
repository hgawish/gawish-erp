using GawishERP.API.Controllers.Base;
using GawishERP.Application.Features.Purchasing.Purchase.Commands.Cancel;
using GawishERP.Application.Features.Purchasing.Purchase.Commands.Create;
using GawishERP.Application.Features.Purchasing.Purchase.Commands.Post;
using GawishERP.Application.Features.Purchasing.Purchase.Queries.GetById;
using GawishERP.Application.Features.Purchasing.Purchase.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

public sealed class PurchasesController : BaseApiController
{
    private readonly IMediator _mediator;

    public PurchasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =====================================================
    // Create Purchase
    // =====================================================

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePurchaseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    // =====================================================
    // Post Purchase
    // =====================================================

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new PostPurchaseCommand(id),
            cancellationToken);

        return Ok(result);
    }

    // =====================================================
    // Cancel Purchase
    // =====================================================

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CancelPurchaseCommand(id),
            cancellationToken);

        return Ok(result);
    }

    // =====================================================
    // Get Purchase By Id
    // =====================================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPurchaseByIdQuery(id),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // =====================================================
    // Get Purchase List
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] GetPurchaseListQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }
}