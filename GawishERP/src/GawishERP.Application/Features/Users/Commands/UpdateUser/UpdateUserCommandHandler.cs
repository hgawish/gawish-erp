using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null)
            return false;

        user.Update(
            request.FirstName,
            request.LastName,
            request.Email);

        await _userRepository.UpdateAsync(user);

        return true;
    }
}