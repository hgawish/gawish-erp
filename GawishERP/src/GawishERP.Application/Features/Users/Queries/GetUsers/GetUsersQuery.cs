using GawishERP.Application.Features.Users.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<List<UserDto>>;