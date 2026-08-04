using GawishERP.API.Controllers.Base;
using GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Cancel;
using GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Create;
using GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Post;
using GawishERP.Application.Features.Purchasing.PurchaseReturn.Queries.GetById;
using GawishERP.Application.Features.Purchasing.PurchaseReturn.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

public sealed class PurchaseReturnsController : BaseApiController
{
    private readonly IMediator _mediator;

    public PurchaseReturnsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ============================================
    // Create Purchase Return
    // ============================================

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePurchaseReturnCommand command,
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

    // ============================================
    // Post Purchase Return
    // ============================================

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new PostPurchaseReturnCommand(id),
            cancellationToken);

        return Ok(result);
    }

    // ============================================
    // Cancel Purchase Return
    // ============================================

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CancelPurchaseReturnCommand(id),
            cancellationToken);

        return Ok(result);
    }

    // ============================================
    // Get Purchase Return By Id
    // ============================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPurchaseReturnByIdQuery(id),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // ============================================
    // Get Purchase Return List
    // ============================================

    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] GetPurchaseReturnListQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }
}