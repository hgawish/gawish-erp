using FluentValidation;

namespace GawishERP.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator
    : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ArabicName)
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .MaximumLength(200)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(1000);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}