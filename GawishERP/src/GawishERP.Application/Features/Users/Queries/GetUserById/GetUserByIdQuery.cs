using GawishERP.Application.Features.Users.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;