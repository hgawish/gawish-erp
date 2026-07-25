using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Users.Commands.DeactivateUser;

public class DeactivateUserCommandHandler
    : IRequestHandler<DeactivateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeactivateUserCommandHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(
        DeactivateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            throw new Exception("User not found.");
        }

        user.Deactivate();

        await _userRepository.UpdateAsync(user);
    }
}