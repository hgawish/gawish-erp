using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class AccountBalance : BaseEntity
{
    public Guid AccountId { get; private set; }

    public Guid FiscalYearId { get; private set; }

    public Guid? CompanyId { get; private set; }

    public Guid? BranchId { get; private set; }

    public decimal OpeningDebit { get; private set; }

    public decimal OpeningCredit { get; private set; }

    public decimal CurrentDebit { get; private set; }

    public decimal CurrentCredit { get; private set; }

    public decimal ClosingBalance { get; private set; }

    public Account Account { get; private set; } = null!;

    public FiscalYear FiscalYear { get; private set; } = null!;

    private AccountBalance()
    {
    }

    public AccountBalance(
        Guid accountId,
        Guid fiscalYearId,
        Guid? companyId = null,
        Guid? branchId = null)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException(nameof(accountId));

        if (fiscalYearId == Guid.Empty)
            throw new ArgumentException(nameof(fiscalYearId));

        AccountId = accountId;

        FiscalYearId = fiscalYearId;

        CompanyId = companyId;

        BranchId = branchId;
    }

    public void SetOpeningBalance(
        decimal openingDebit,
        decimal openingCredit)
    {
        if (openingDebit < 0)
            throw new ArgumentException(nameof(openingDebit));

        if (openingCredit < 0)
            throw new ArgumentException(nameof(openingCredit));

        if (openingDebit > 0 && openingCredit > 0)
            throw new InvalidOperationException(
                "Opening balance cannot contain both Debit and Credit.");

        OpeningDebit += openingDebit;

        OpeningCredit += openingCredit;

        Recalculate();
    }

    public void ApplyTransaction(
        decimal debit,
        decimal credit)
    {
        if (debit < 0)
            throw new ArgumentException(nameof(debit));

        if (credit < 0)
            throw new ArgumentException(nameof(credit));

        if (debit > 0 && credit > 0)
            throw new InvalidOperationException(
                "Transaction cannot contain both Debit and Credit.");

        CurrentDebit += debit;

        CurrentCredit += credit;

        Recalculate();
    }

    private void Recalculate()
    {
        ClosingBalance =
            (OpeningDebit + CurrentDebit)
            - (OpeningCredit + CurrentCredit);
    }
}