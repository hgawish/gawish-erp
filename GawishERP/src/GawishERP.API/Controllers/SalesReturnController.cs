using GawishERP.Application.Features.Sales.SalesReturn.Commands.Cancel;
using GawishERP.Application.Features.Sales.SalesReturn.Commands.Create;
using GawishERP.Application.Features.Sales.SalesReturn.Commands.Post;
using GawishERP.Application.Features.Sales.SalesReturn.Queries.GetById;
using GawishERP.Application.Features.Sales.SalesReturn.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SalesReturnController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesReturnController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create Sales Return
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateSalesReturnResponse>> Create(
        CreateSalesReturnCommand command,
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
    /// Get Sales Return By Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetSalesReturnByIdResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSalesReturnByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get Sales Return List
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetSalesReturnListResponse>> GetList(
        [FromQuery] GetSalesReturnListQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Post Sales Return
    /// </summary>
    [HttpPost("{id:guid}/post")]
    public async Task<ActionResult<PostSalesReturnResponse>> Post(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new PostSalesReturnCommand(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Cancel Sales Return
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<CancelSalesReturnResponse>> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CancelSalesReturnCommand(id),
            cancellationToken);

        return Ok(result);
    }
}