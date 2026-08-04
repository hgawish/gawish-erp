using GawishERP.API.Authorization;
using GawishERP.API.Controllers.Base;
using GawishERP.Application.Features.Customers.Commands.CreateCustomer;
using GawishERP.Application.Features.Customers.Commands.UpdateCustomer;
using GawishERP.Application.Features.Customers.Queries.GetAllCustomers;
using GawishERP.Application.Features.Customers.Queries.GetCustomerById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : BaseApiController
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ============================================
    // GET ALL CUSTOMERS
    // ============================================

    [HttpGet]
    [HasPermission("Customers.View")]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllCustomersQuery query)
    {
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    // ============================================
    // GET CUSTOMER BY ID
    // ============================================

    [HttpGet("{id:guid}")]
    [HasPermission("Customers.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(
            new GetCustomerByIdQuery(id));

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    // ============================================
    // CREATE CUSTOMER
    // ============================================

    [HttpPost]
    [HasPermission("Customers.Create")]
    public async Task<IActionResult> Create(
        CreateCustomerCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(new
        {
            Id = id,
            Message = "Customer created successfully."
        });
    }

    // ============================================
    // UPDATE CUSTOMER
    // ============================================

    [HttpPut("{id:guid}")]
    [HasPermission("Customers.Edit")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCustomerCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(
                "Route Id does not match request Id.");
        }

        var customerId = await _mediator.Send(command);

        return Ok(new
        {
            Id = customerId,
            Message = "Customer updated successfully."
        });
    }
}