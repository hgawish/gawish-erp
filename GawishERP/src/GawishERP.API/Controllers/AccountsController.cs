using GawishERP.Application.Features.Accounting.Accounts.Commands.Activate;
using GawishERP.Application.Features.Accounting.Accounts.Commands.Create;
using GawishERP.Application.Features.Accounting.Accounts.Commands.Deactivate;
using GawishERP.Application.Features.Accounting.Accounts.Commands.Update;
using GawishERP.Application.Features.Accounting.Accounts.Queries.GetById;
using GawishERP.Application.Features.Accounting.Accounts.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateAccountResponse>> Create(
        CreateAccountCommand command,
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

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UpdateAccountResponse>> Update(
        Guid id,
        UpdateAccountCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest();

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetAccountByIdResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAccountByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<GetAccountsListResponse>> GetList(
        [FromQuery] GetAccountsListQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<ActionResult<ActivateAccountResponse>> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ActivateAccountCommand(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<DeactivateAccountResponse>> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeactivateAccountCommand(id),
            cancellationToken);

        return Ok(result);
    }
}