using FluentValidation;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Create;

public sealed class CreatePurchaseCommandValidator
    : AbstractValidator<CreatePurchaseCommand>
{
    public CreatePurchaseCommandValidator()
    {
        RuleFor(x => x.InvoiceNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.SupplierId)
            .NotEmpty();

        RuleFor(x => x.WarehouseId)
            .NotEmpty();

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.ExchangeRate)
            .GreaterThan(0);

        RuleFor(x => x.Lines)
            .NotEmpty();

        RuleForEach(x => x.Lines)
            .SetValidator(new CreatePurchaseLineValidator());
    }
}

internal sealed class CreatePurchaseLineValidator
    : AbstractValidator<CreatePurchaseLineDto>
{
    public CreatePurchaseLineValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.TaxAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.BatchNumber)
            .NotEmpty()
            .MaximumLength(100);
    }
}