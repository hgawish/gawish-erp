using GawishERP.Application.Features.Sales.SalesDeliveries.Commands.ApproveSalesDelivery;
using GawishERP.Application.Features.Sales.SalesDeliveries.Commands.CancelSalesDelivery;
using GawishERP.Application.Features.Sales.SalesDeliveries.Commands.CreateSalesDelivery;
using GawishERP.Application.Features.Sales.SalesDeliveries.Commands.PostSalesDelivery;
using GawishERP.Application.Features.Sales.SalesDeliveries.Commands.SubmitSalesDelivery;
using GawishERP.Application.Features.Sales.SalesDeliveries.Queries.GetSalesDeliveries;
using GawishERP.Application.Features.Sales.SalesDeliveries.Queries.GetSalesDeliveryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SalesDeliveriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesDeliveriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSalesDeliveriesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSalesDeliveryByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSalesDeliveryCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SubmitSalesDeliveryCommand(id), cancellationToken);
        return Ok(new { id = result, status = "Submitted" });
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ApproveSalesDeliveryCommand(id), cancellationToken);
        return Ok(new { id = result, status = "Approved" });
    }

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new PostSalesDeliveryCommand(id), cancellationToken);
        return Ok(new { id = result, status = "Posted" });
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CancelSalesDeliveryCommand(id),
            cancellationToken);

        return Ok(result);
    }
}