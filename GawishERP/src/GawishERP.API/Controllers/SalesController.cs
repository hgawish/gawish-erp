using GawishERP.Application.Features.Sales.Sales.Commands.Approve;
using GawishERP.Application.Features.Sales.Sales.Commands.Cancel;
using GawishERP.Application.Features.Sales.Sales.Commands.Create;
using GawishERP.Application.Features.Sales.Sales.Commands.Post;
using GawishERP.Application.Features.Sales.Sales.Commands.Submit;
using GawishERP.Application.Features.Sales.Sales.Queries.GetById;
using GawishERP.Application.Features.Sales.Sales.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create Sales Invoice
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateSalesResponse>> Create(
        CreateSalesCommand command,
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

    /// <summary>
    /// Get Sales By Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetSalesByIdResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSalesByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get Sales List
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetSalesListResponse>> GetList(
        [FromQuery] GetSalesListQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Submit Sales Invoice
    /// </summary>
    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SubmitSalesCommand(id),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result);
    }

    /// <summary>
    /// Approve Sales Invoice
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ApproveSalesCommand(id),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result);
    }

    /// <summary>
    /// Post Sales Invoice
    /// </summary>
    [HttpPost("{id:guid}/post")]
    public async Task<ActionResult<PostSalesResponse>> Post(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new PostSalesCommand(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Cancel Sales Invoice
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<CancelSalesResponse>> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CancelSalesCommand(id),
            cancellationToken);

        return Ok(result);
    }
}