using GawishERP.Application.Features.Users.Commands.ActivateUser;
using GawishERP.Application.Features.Users.Commands.CreateUser;
using GawishERP.Application.Features.Users.Commands.DeactivateUser;
using GawishERP.Application.Features.Users.Commands.DeleteUser;
using GawishERP.Application.Features.Users.Commands.UpdateUser;
using GawishERP.Application.Features.Users.Queries.GetUserById;
using GawishERP.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _mediator.Send(new GetUsersQuery());

        return Ok(users);
    }

    // GET: api/Users/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));

        if (user is null)
        {
            return NotFound(new
            {
                Message = "User not found."
            });
        }

        return Ok(user);
    }

    // POST: api/Users
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(new
        {
            Message = "User created successfully.",
            UserId = id
        });
    }

    // PUT: api/Users/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new
            {
                Message = "Route Id does not match Request Id."
            });
        }

        await _mediator.Send(command);

        return Ok(new
        {
            Message = "User updated successfully."
        });
    }

    // PUT: api/Users/{id}/activate
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _mediator.Send(new ActivateUserCommand(id));

        return Ok(new
        {
            Message = "User activated successfully."
        });
    }

    // PUT: api/Users/{id}/deactivate
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _mediator.Send(new DeactivateUserCommand(id));

        return Ok(new
        {
            Message = "User deactivated successfully."
        });
    }

    // DELETE: api/Users/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));

        return Ok(new
        {
            Message = "User deleted successfully."
        });
    }
}