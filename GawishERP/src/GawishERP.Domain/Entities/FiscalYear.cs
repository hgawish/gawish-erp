using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class FiscalYear : ActivatableEntity
{
    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public bool IsOpen { get; private set; }

    public bool IsClosed { get; private set; }

    private FiscalYear()
    {
    }

    public FiscalYear(
        string code,
        string name,
        DateOnly startDate,
        DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Fiscal year code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Fiscal year name is required.", nameof(name));

        if (endDate < startDate)
            throw new ArgumentException("End date cannot be before start date.");

        Code = code.Trim();
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;

        IsOpen = true;
        IsClosed = false;
    }

    public void Update(
        string name,
        DateOnly startDate,
        DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Fiscal year name is required.", nameof(name));

        if (endDate < startDate)
            throw new ArgumentException("End date cannot be before start date.");

        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }

    public void Open()
    {
        if (IsOpen)
            throw new InvalidOperationException("Fiscal year is already open.");

        IsOpen = true;
        IsClosed = false;
    }

    public void Close()
    {
        if (IsClosed)
            throw new InvalidOperationException("Fiscal year is already closed.");

        IsClosed = true;
        IsOpen = false;
    }

    public void ReOpen()
    {
        if (!IsClosed)
            throw new InvalidOperationException("Fiscal year is not closed.");

        IsClosed = false;
        IsOpen = true;
    }
}