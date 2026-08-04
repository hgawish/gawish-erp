using FluentValidation;

namespace GawishERP.Application.Features.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandValidator
    : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ArabicName)
            .MaximumLength(200);

        RuleFor(x => x.ContactPerson)
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .MaximumLength(50);

        RuleFor(x => x.Mobile)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .MaximumLength(200);

        RuleFor(x => x.TaxNumber)
            .MaximumLength(100);

        RuleFor(x => x.CommercialRegistration)
            .MaximumLength(100);

        RuleFor(x => x.Address)
            .MaximumLength(1000);

        RuleFor(x => x.City)
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .MaximumLength(100);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}