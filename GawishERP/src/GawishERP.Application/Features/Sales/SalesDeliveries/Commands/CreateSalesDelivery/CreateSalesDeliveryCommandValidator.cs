using FluentValidation;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.CreateSalesDelivery;

public sealed class CreateSalesDeliveryCommandValidator
    : AbstractValidator<CreateSalesDeliveryCommand>
{
    public CreateSalesDeliveryCommandValidator()
    {
        RuleFor(x => x.SalesOrderId)
            .NotEmpty()
            .WithMessage("Sales Order is required.");

        RuleFor(x => x.DocumentDate)
            .NotEmpty()
            .WithMessage("Document date is required.");

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Sales Delivery must contain at least one line.");

        RuleForEach(x => x.Lines)
            .SetValidator(new CreateSalesDeliveryLineValidator());
    }
}

public sealed class CreateSalesDeliveryLineValidator
    : AbstractValidator<CreateSalesDeliveryLineDto>
{
    public CreateSalesDeliveryLineValidator()
    {
        RuleFor(x => x.SalesOrderLineId)
            .NotEmpty()
            .WithMessage("Sales Order Line is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Delivery quantity must be greater than zero.");
    }
}