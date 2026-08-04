using FluentValidation;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Create;

public sealed class CreateJournalEntryValidator
    : AbstractValidator<CreateJournalEntryCommand>
{
    public CreateJournalEntryValidator()
    {
        RuleFor(x => x.JournalEntry)
            .NotNull();

        RuleFor(x => x.JournalEntry.JournalDate)
            .NotEmpty();

        RuleFor(x => x.JournalEntry.FiscalYearId)
            .NotEmpty();

        RuleFor(x => x.JournalEntry.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.JournalEntry.Lines)
            .NotEmpty()
            .Must(x => x.Count >= 2)
            .WithMessage("Journal Entry must contain at least two lines.");

        RuleForEach(x => x.JournalEntry.Lines)
            .ChildRules(line =>
            {
                line.RuleFor(x => x.AccountId)
                    .NotEmpty();

                line.RuleFor(x => x.Debit)
                    .GreaterThanOrEqualTo(0);

                line.RuleFor(x => x.Credit)
                    .GreaterThanOrEqualTo(0);

                line.RuleFor(x => x)
                    .Must(x =>
                        (x.Debit > 0 && x.Credit == 0) ||
                        (x.Credit > 0 && x.Debit == 0))
                    .WithMessage("Each line must contain either Debit or Credit only.");
            });

        RuleFor(x => x.JournalEntry.Lines)
            .Must(lines =>
            {
                var debit = lines.Sum(x => x.Debit);
                var credit = lines.Sum(x => x.Credit);

                return debit == credit;
            })
            .WithMessage("Total Debit must equal Total Credit.");
    }
}