using GawishERP.Application.Features.Accounting.JournalEntries.Commands.Approve;
using GawishERP.Application.Features.Accounting.JournalEntries.Commands.Create;
using GawishERP.Application.Features.Accounting.JournalEntries.Commands.Post;
using GawishERP.Application.Features.Accounting.JournalEntries.Commands.Reverse;
using GawishERP.Application.Features.Accounting.JournalEntries.Commands.Submit;
using GawishERP.Application.Features.Accounting.JournalEntries.DTOs;
using GawishERP.Application.Features.Accounting.JournalEntries.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/journal-entries")]
public sealed class JournalEntriesController : ControllerBase
{
    private readonly ISender _sender;

    public JournalEntriesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create Journal Entry
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateJournalEntryDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateJournalEntryCommand(dto);

        var result = await _sender.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get Journal Entry By Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetJournalEntryByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Submit Journal Entry
    /// </summary>
    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SubmitJournalEntryCommand(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Approve Journal Entry
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ApproveJournalEntryCommand(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Post Journal Entry
    /// </summary>
    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new PostJournalEntryCommand(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Reverse Journal Entry
    /// </summary>
    [HttpPost("{id:guid}/reverse")]
    public async Task<IActionResult> Reverse(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ReverseJournalEntryCommand(id),
            cancellationToken);

        return Ok(result);
    }
}