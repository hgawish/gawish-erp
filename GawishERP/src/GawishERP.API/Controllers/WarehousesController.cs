using GawishERP.API.Authorization;
using GawishERP.API.Controllers.Base;
using GawishERP.Application.Features.Warehouses.Commands.ActivateWarehouse;
using GawishERP.Application.Features.Warehouses.Commands.CreateWarehouse;
using GawishERP.Application.Features.Warehouses.Commands.DeactivateWarehouse;
using GawishERP.Application.Features.Warehouses.Commands.UpdateWarehouse;
using GawishERP.Application.Features.Warehouses.Queries.GetAllWarehouses;
using GawishERP.Application.Features.Warehouses.Queries.GetWarehouseById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : BaseApiController
{
    private readonly IMediator _mediator;

    public WarehousesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ============================================
    // GET ALL
    // ============================================

    [HttpGet]
    [HasPermission("Warehouses.View")]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllWarehousesQuery query)
    {
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    // ============================================
    // GET BY ID
    // ============================================

    [HttpGet("{id:guid}")]
    [HasPermission("Warehouses.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(
            new GetWarehouseByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // ============================================
    // CREATE
    // ============================================

    [HttpPost]
    [HasPermission("Warehouses.Create")]
    public async Task<IActionResult> Create(
        CreateWarehouseCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(new
        {
            Id = id,
            Message = "Warehouse created successfully."
        });
    }

    // ============================================
    // UPDATE
    // ============================================

    [HttpPut("{id:guid}")]
    [HasPermission("Warehouses.Edit")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateWarehouseCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        var warehouseId = await _mediator.Send(command);

        return Ok(new
        {
            Id = warehouseId,
            Message = "Warehouse updated successfully."
        });
    }

    // ============================================
    // ACTIVATE
    // ============================================

    [HttpPatch("{id:guid}/activate")]
    [HasPermission("Warehouses.Edit")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _mediator.Send(new ActivateWarehouseCommand(id));

        return Ok(new
        {
            Message = "Warehouse activated successfully."
        });
    }

    // ============================================
    // DEACTIVATE
    // ============================================

    [HttpPatch("{id:guid}/deactivate")]
    [HasPermission("Warehouses.Delete")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _mediator.Send(new DeactivateWarehouseCommand(id));

        return Ok(new
        {
            Message = "Warehouse deactivated successfully."
        });
    }
}