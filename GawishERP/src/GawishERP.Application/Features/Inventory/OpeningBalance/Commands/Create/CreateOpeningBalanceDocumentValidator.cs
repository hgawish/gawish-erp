using FluentValidation;
using GawishERP.Application.Features.Inventory.OpeningBalance.Commands.CreateOpeningBalanceDocument;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Create;

public class CreateOpeningBalanceDocumentValidator
    : AbstractValidator<CreateOpeningBalanceDocumentCommand>
{
    public CreateOpeningBalanceDocumentValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty()
            .WithMessage("Warehouse is required.");

        RuleFor(x => x.DocumentDate)
            .NotEmpty()
            .WithMessage("Document date is required.");

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Opening Balance must contain at least one line.");

        RuleForEach(x => x.Lines)
            .SetValidator(new CreateOpeningBalanceLineValidator());
    }
}

public class CreateOpeningBalanceLineValidator
    : AbstractValidator<CreateOpeningBalanceLineDto>
{
    public CreateOpeningBalanceLineValidator()
    {
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