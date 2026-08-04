using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class NumberSeries : BaseEntity
{
    public DocumentType DocumentType { get; private set; }

    public Guid? CompanyId { get; private set; }

    public Guid? BranchId { get; private set; }

    public Guid? FiscalYearId { get; private set; }

    public string Prefix { get; private set; } = string.Empty;

    public int CurrentNumber { get; private set; }

    public int Padding { get; private set; }

    public bool IsActive { get; private set; }

    public byte[] RowVersion { get; private set; } = default!;

    private NumberSeries()
    {
    }

    public NumberSeries(
        DocumentType documentType,
        string prefix,
        Guid? companyId = null,
        Guid? branchId = null,
        Guid? fiscalYearId = null,
        int currentNumber = 0,
        int padding = 6)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix is required.", nameof(prefix));

        if (padding <= 0)
            throw new ArgumentException("Padding must be greater than zero.", nameof(padding));

        DocumentType = documentType;

        CompanyId = companyId;

        BranchId = branchId;

        FiscalYearId = fiscalYearId;

        Prefix = prefix.Trim().ToUpperInvariant();

        CurrentNumber = currentNumber;

        Padding = padding;

        IsActive = true;
    }

    public string GenerateNextNumber()
    {
        if (!IsActive)
            throw new InvalidOperationException("Number Series is inactive.");

        CurrentNumber++;

        return $"{Prefix}-{CurrentNumber.ToString().PadLeft(Padding, '0')}";
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void ChangePrefix(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix is required.", nameof(prefix));

        Prefix = prefix.Trim().ToUpperInvariant();
    }

    public void ChangePadding(int padding)
    {
        if (padding <= 0)
            throw new ArgumentException("Padding must be greater than zero.", nameof(padding));

        Padding = padding;
    }

    public void SetCurrentNumber(int currentNumber)
    {
        if (currentNumber < 0)
            throw new ArgumentException("Current Number cannot be negative.", nameof(currentNumber));

        CurrentNumber = currentNumber;
    }

    public void ChangeScope(
        Guid? companyId,
        Guid? branchId,
        Guid? fiscalYearId)
    {
        CompanyId = companyId;
        BranchId = branchId;
        FiscalYearId = fiscalYearId;
    }
}