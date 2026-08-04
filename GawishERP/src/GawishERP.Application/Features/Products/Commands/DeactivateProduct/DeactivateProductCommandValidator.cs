using FluentValidation;

namespace GawishERP.Application.Features.Products.Commands.DeactivateProduct;

public class DeactivateProductCommandValidator
    : AbstractValidator<DeactivateProductCommand>
{
    public DeactivateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}