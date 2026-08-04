using GawishERP.API.Authorization;
using GawishERP.API.Controllers.Base;
using GawishERP.Application.Features.FiscalYears.Commands.Create;
using GawishERP.Application.Features.FiscalYears.Commands.Update;
using GawishERP.Application.Features.FiscalYears.Queries.GetAllFiscalYears;
using GawishERP.Application.Features.FiscalYears.Queries.GetFiscalYearById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FiscalYearsController : BaseApiController
{
    private readonly IMediator _mediator;

    public FiscalYearsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ============================================
    // GET ALL
    // ============================================

    [HttpGet]
    [HasPermission("FiscalYears.View")]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllFiscalYearsQuery query)
    {
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    // ============================================
    // GET BY ID
    // ============================================

    [HttpGet("{id:guid}")]
    [HasPermission("FiscalYears.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(
            new GetFiscalYearByIdQuery(id));

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    // ============================================
    // CREATE
    // ============================================

    [HttpPost]
    [HasPermission("FiscalYears.Create")]
    public async Task<IActionResult> Create(
        CreateFiscalYearCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(new
        {
            Id = id,
            Message = "Fiscal Year created successfully."
        });
    }

    // ============================================
    // UPDATE
    // ============================================

    [HttpPut("{id:guid}")]
    [HasPermission("FiscalYears.Edit")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateFiscalYearCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(
                "Route Id does not match request Id.");
        }

        var fiscalYearId =
            await _mediator.Send(command);

        return Ok(new
        {
            Id = fiscalYearId,
            Message = "Fiscal Year updated successfully."
        });
    }
}