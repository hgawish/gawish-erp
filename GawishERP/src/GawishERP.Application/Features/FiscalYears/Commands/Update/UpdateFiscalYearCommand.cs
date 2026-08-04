using MediatR;

namespace GawishERP.Application.Features.FiscalYears.Commands.Update;

public sealed class UpdateFiscalYearCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; }
}