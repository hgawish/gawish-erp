using FluentValidation;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Cancel;

public sealed class CancelPurchaseValidator
    : AbstractValidator<CancelPurchaseCommand>
{
    public CancelPurchaseValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty();
    }
}