using GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Create;
using GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Post;
using GawishERP.Application.Features.Accounting.OpeningBalances.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AccountingOpeningBalancesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountingOpeningBalancesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //=========================================================
    // Create Accounting Opening Balance
    //=========================================================

    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] CreateOpeningBalanceCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    //=========================================================
    // Get Accounting Opening Balance By Id
    //=========================================================

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetOpeningBalanceByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    //=========================================================
    // Post Accounting Opening Balance
    //=========================================================

    [HttpPost("{id:guid}/post")]
    public async Task<ActionResult> Post(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new PostOpeningBalanceCommand(id),
            cancellationToken);

        return Ok(result);
    }
}