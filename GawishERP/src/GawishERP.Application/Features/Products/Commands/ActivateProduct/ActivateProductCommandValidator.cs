using FluentValidation;

namespace GawishERP.Application.Features.Products.Commands.ActivateProduct;

public class ActivateProductCommandValidator
    : AbstractValidator<ActivateProductCommand>
{
    public ActivateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}