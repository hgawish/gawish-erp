using MediatR;

namespace GawishERP.Application.Features.Users.Commands.ActivateUser;

public record ActivateUserCommand(Guid Id) : IRequest;