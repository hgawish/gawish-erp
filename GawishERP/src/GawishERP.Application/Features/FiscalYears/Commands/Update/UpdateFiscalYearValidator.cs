using FluentValidation;

namespace GawishERP.Application.Features.FiscalYears.Commands.Update;

public sealed class UpdateFiscalYearValidator
    : AbstractValidator<UpdateFiscalYearCommand>
{
    public UpdateFiscalYearValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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