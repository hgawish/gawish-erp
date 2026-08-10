using GawishERP.Application.Features.Sales.SalesOrders.Commands.ApproveSalesOrder;
using GawishERP.Application.Features.Sales.SalesOrders.Commands.CancelSalesOrder;
using GawishERP.Application.Features.Sales.SalesOrders.Commands.CreateSalesOrder;
using GawishERP.Application.Features.Sales.SalesOrders.Commands.PostSalesOrder;
using GawishERP.Application.Features.Sales.SalesOrders.Commands.SubmitSalesOrder;
using GawishERP.Application.Features.Sales.SalesOrders.Queries.GetSalesOrderById;
using GawishERP.Application.Features.Sales.SalesOrders.Queries.GetSalesOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SalesOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //=========================================================
    // GET: api/SalesOrders
    //=========================================================

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSalesOrdersQuery(),
            cancellationToken);

        return Ok(result);
    }

    //=========================================================
    // GET: api/SalesOrders/{id}
    //=========================================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSalesOrderByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    //=========================================================
    // POST: api/SalesOrders
    //=========================================================

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSalesOrderCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new { id });
    }

    //=========================================================
    // POST: api/SalesOrders/{id}/submit
    //=========================================================

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SubmitSalesOrderCommand(id),
            cancellationToken);

        return Ok(new { id = result });
    }

    //=========================================================
    // POST: api/SalesOrders/{id}/approve
    //=========================================================

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ApproveSalesOrderCommand(id),
            cancellationToken);

        return Ok(new { id = result });
    }

    //=========================================================
    // POST: api/SalesOrders/{id}/post
    //=========================================================

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new PostSalesOrderCommand(id),
            cancellationToken);

        return Ok(new { id = result });
    }

    //=========================================================
    // POST: api/SalesOrders/{id}/cancel
    //=========================================================

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CancelSalesOrderCommand(id),
            cancellationToken);

        return Ok(new { id = result });
    }
}