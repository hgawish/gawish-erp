using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.FiscalYears.Commands.Create;

public sealed class CreateFiscalYearCommandHandler
    : IRequestHandler<CreateFiscalYearCommand, Guid>
{
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFiscalYearCommandHandler(
        IFiscalYearRepository fiscalYearRepository,
        IUnitOfWork unitOfWork)
    {
        _fiscalYearRepository = fiscalYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateFiscalYearCommand request,
        CancellationToken cancellationToken)
    {
        var existing =
            await _fiscalYearRepository.GetByCodeAsync(request.Code);

        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"Fiscal Year code '{request.Code}' already exists.");
        }

        var fiscalYear = new FiscalYear(
            request.Code,
            request.Name,
            request.StartDate,
            request.EndDate);

        _fiscalYearRepository.Add(fiscalYear);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return fiscalYear.Id;
    }
}