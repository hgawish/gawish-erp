using FluentValidation;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Post;

public sealed class PostPurchaseCommandValidator
    : AbstractValidator<PostPurchaseCommand>
{
    public PostPurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty();
    }
}