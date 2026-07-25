using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Users.Commands.ActivateUser;

public class ActivateUserCommandHandler
    : IRequestHandler<ActivateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public ActivateUserCommandHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(
        ActivateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            throw new Exception("User not found.");
        }

        user.Activate();

        await _userRepository.UpdateAsync(user);
    }
}