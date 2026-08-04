using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.FiscalYears.Commands.Update;

public sealed class UpdateFiscalYearCommandHandler
    : IRequestHandler<UpdateFiscalYearCommand, Guid>
{
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFiscalYearCommandHandler(
        IFiscalYearRepository fiscalYearRepository,
        IUnitOfWork unitOfWork)
    {
        _fiscalYearRepository = fiscalYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        UpdateFiscalYearCommand request,
        CancellationToken cancellationToken)
    {
        var fiscalYear =
            await _fiscalYearRepository.GetByIdAsync(request.Id);

        if (fiscalYear is null)
        {
            throw new InvalidOperationException(
                $"Fiscal Year '{request.Id}' was not found.");
        }

        fiscalYear.Update(
            request.Name,
            request.StartDate,
            request.EndDate);

        if (request.IsActive)
            fiscalYear.Activate();
        else
            fiscalYear.Deactivate();

        _fiscalYearRepository.Update(fiscalYear);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return fiscalYear.Id;
    }
}