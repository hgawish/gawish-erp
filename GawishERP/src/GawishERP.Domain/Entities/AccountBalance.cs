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
        AccountId = accountId;
        FiscalYearId = fiscalYearId;
        CompanyId = companyId;
        BranchId = branchId;
    }

    public void SetOpeningBalance(
        decimal openingDebit,
        decimal openingCredit)
    {
        OpeningDebit = openingDebit;
        OpeningCredit = openingCredit;

        Recalculate();
    }

    public void ApplyTransaction(
        decimal debit,
        decimal credit)
    {
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