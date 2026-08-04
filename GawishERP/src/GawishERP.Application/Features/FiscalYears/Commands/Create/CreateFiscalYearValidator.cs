using FluentValidation;

namespace GawishERP.Application.Features.FiscalYears.Commands.Create;

public sealed class CreateFiscalYearValidator
    : AbstractValidator<CreateFiscalYearCommand>
{
    public CreateFiscalYearValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.EndDate >= x.StartDate)
            .WithMessage("End Date must be greater than or equal to Start Date.");
    }
}