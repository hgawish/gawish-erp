using GawishERP.API.Controllers.Base;
using GawishERP.Application.Features.Inventory.OpeningBalance.Commands.CreateOpeningBalanceDocument;
using GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Post;
using GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Submit;
using GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Approve;
using GawishERP.Application.Features.Inventory.OpeningBalance.Queries.GetById;
using GawishERP.Application.Features.Inventory.OpeningBalance.Queries.GetList;
using GawishERP.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

public sealed class OpeningBalancesController : BaseApiController
{
    private readonly IMediator _mediator;

    public OpeningBalancesController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    //=========================================================
    // GET: api/OpeningBalances/enum-test
    //=========================================================

    [HttpGet("enum-test")]
    public IActionResult EnumTest()
    {
        return Ok(new
        {
            Draft = (int)DocumentStatus.Draft,
            Submitted = (int)DocumentStatus.Submitted,
            Approved = (int)DocumentStatus.Approved,
            Posted = (int)DocumentStatus.Posted,
            Cancelled = (int)DocumentStatus.Cancelled
        });
    }

    //=========================================================
    // POST: api/OpeningBalances
    //=========================================================

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
            new { id });
    }

    //=========================================================
    // POST: api/OpeningBalances/{id}/submit
    //=========================================================

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new SubmitOpeningBalanceCommand
            {
                Id = id
            },
            cancellationToken);

        return Ok(new
        {
            Message = "Opening Balance submitted successfully."
        });
    }

    //=========================================================
    // POST: api/OpeningBalances/{id}/approve
    //=========================================================

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ApproveOpeningBalanceCommand
            {
                Id = id
            },
            cancellationToken);

        return Ok(new
        {
            Message = "Opening Balance approved successfully."
        });
    }

    //=========================================================
    // POST: api/OpeningBalances/{id}/post
    //=========================================================

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

    //=========================================================
    // GET: api/OpeningBalances/{id}
    //=========================================================

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

    //=========================================================
    // GET: api/OpeningBalances
    //=========================================================

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