using GawishERP.Application.Features.Inventory.Valuation.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class InventoryValuationController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryValuationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetInventoryValuationResponse>> Get(
        [FromQuery] Guid? productId,
        [FromQuery] Guid? warehouseId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetInventoryValuationQuery(productId, warehouseId),
            cancellationToken);

        return Ok(result);
    }
}