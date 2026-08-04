using FluentValidation;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Create;

public sealed class CreatePurchaseReturnCommandValidator
    : AbstractValidator<CreatePurchaseReturnCommand>
{
    public CreatePurchaseReturnCommandValidator()
    {
        RuleFor(x => x.DocumentDate)
            .NotEmpty()
            .WithMessage("Document Date is required.");

        RuleFor(x => x.PurchaseId)
            .NotEmpty()
            .WithMessage("Purchase is required.");

        RuleFor(x => x.SupplierId)
            .NotEmpty()
            .WithMessage("Supplier is required.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty()
            .WithMessage("Warehouse is required.");

        RuleFor(x => x.ReturnReason)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("At least one return line is required.");

        RuleForEach(x => x.Lines)
            .SetValidator(new CreatePurchaseReturnLineValidator());
    }
}

public sealed class CreatePurchaseReturnLineValidator
    : AbstractValidator<CreatePurchaseReturnLineDto>
{
    public CreatePurchaseReturnLineValidator()
    {
        RuleFor(x => x.PurchaseLineId)
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }
}