using FluentValidation;

namespace GawishERP.Application.Features.Warehouses.Commands.UpdateWarehouse;

public class UpdateWarehouseCommandValidator
    : AbstractValidator<UpdateWarehouseCommand>
{
    public UpdateWarehouseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ArabicName)
            .MaximumLength(200);

        RuleFor(x => x.Manager)
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .MaximumLength(50);

        RuleFor(x => x.Address)
            .MaximumLength(1000);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}