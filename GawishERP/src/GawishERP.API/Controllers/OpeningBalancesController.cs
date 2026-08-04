using GawishERP.API.Controllers.Base;
using GawishERP.Application.Features.Inventory.OpeningBalance.Commands.CreateOpeningBalanceDocument;
using GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Post;
using GawishERP.Application.Features.Inventory.OpeningBalance.Queries.GetById;
using GawishERP.Application.Features.Inventory.OpeningBalance.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GawishERP.Domain.Common;

namespace GawishERP.API.Controllers;

public sealed class OpeningBalancesController : BaseApiController
{
    [HttpGet("enum-test")]
    public IActionResult EnumTest()
    {
        return Ok(new
        {
            Draft = (int)DocumentStatus.Draft,
            Posted = (int)DocumentStatus.Posted,
            Cancelled = (int)DocumentStatus.Cancelled
        });
    }

    private readonly IMediator _mediator;

    public OpeningBalancesController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOpeningBalanceDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id);
    }

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new PostOpeningBalanceCommand
            {
                Id = id
            },
            cancellationToken);

        return Ok(new
        {
            Message = "Opening Balance posted successfully."
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetOpeningBalanceByIdQuery
            {
                Id = id
            },
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] GetOpeningBalanceListQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }
}