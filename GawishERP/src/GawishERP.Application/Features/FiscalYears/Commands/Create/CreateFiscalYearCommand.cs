using MediatR;

namespace GawishERP.Application.Features.FiscalYears.Commands.Create;

public sealed class CreateFiscalYearCommand : IRequest<Guid>
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }
}