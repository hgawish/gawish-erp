using GawishERP.API.Authorization;
using GawishERP.API.Controllers.Base;
using GawishERP.Application.Features.Suppliers.Commands.CreateSupplier;
using GawishERP.Application.Features.Suppliers.Commands.UpdateSupplier;
using GawishERP.Application.Features.Suppliers.Queries.GetAllSuppliers;
using GawishERP.Application.Features.Suppliers.Queries.GetSupplierById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : BaseApiController
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ============================================
    // GET ALL SUPPLIERS
    // ============================================

    [HttpGet]
    [HasPermission("Suppliers.View")]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllSuppliersQuery query)
    {
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    // ============================================
    // GET SUPPLIER BY ID
    // ============================================

    [HttpGet("{id:guid}")]
    [HasPermission("Suppliers.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(
            new GetSupplierByIdQuery(id));

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    // ============================================
    // CREATE SUPPLIER
    // ============================================

    [HttpPost]
    [HasPermission("Suppliers.Create")]
    public async Task<IActionResult> Create(
        CreateSupplierCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(new
        {
            Id = id,
            Message = "Supplier created successfully."
        });
    }

    // ============================================
    // UPDATE SUPPLIER
    // ============================================

    [HttpPut("{id:guid}")]
    [HasPermission("Suppliers.Edit")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateSupplierCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(
                "Route Id does not match request Id.");
        }

        var supplierId = await _mediator.Send(command);

        return Ok(new
        {
            Id = supplierId,
            Message = "Supplier updated successfully."
        });
    }
}