using FluentValidation;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Post;

public sealed class PostOpeningBalanceCommandValidator
    : AbstractValidator<PostOpeningBalanceCommand>
{
    public PostOpeningBalanceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}