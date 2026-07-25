using GawishERP.API.Authorization;
using GawishERP.Application.Features.Products.Commands.CreateProduct;
using GawishERP.Application.Features.Products.Commands.UpdateProduct;
using GawishERP.Application.Features.Products.Queries.GetAllProducts;
using GawishERP.Application.Features.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GawishERP.API.Controllers.Base;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ============================================
    // GET ALL PRODUCTS
    // ============================================

    [HttpGet]
    [HasPermission("Products.View")]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllProductsQuery query)
    {
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    // ============================================
    // GET PRODUCT BY ID
    // ============================================

    [HttpGet("{id:guid}")]
    [HasPermission("Products.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    // ============================================
    // CREATE PRODUCT
    // ============================================

    [HttpPost]
    [HasPermission("Products.Create")]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(new
        {
            Id = id,
            Message = "Product created successfully."
        });
    }

    // ============================================
    // UPDATE PRODUCT
    // ============================================

    [HttpPut("{id:guid}")]
    [HasPermission("Products.Edit")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route Id does not match request Id.");
        }

        var productId = await _mediator.Send(command);

        return Ok(new
        {
            Id = productId,
            Message = "Product updated successfully."
        });
    }
}