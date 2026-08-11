using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Approve;

public sealed class ApproveSalesCommandHandler
    : IRequestHandler<ApproveSalesCommand, Result>
{
    private readonly ISalesRepository _salesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveSalesCommandHandler(
        ISalesRepository salesRepository,
        IUnitOfWork unitOfWork)
    {
        _salesRepository = salesRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ApproveSalesCommand request,
        CancellationToken cancellationToken)
    {
        var sales = await _salesRepository.GetByIdAsync(
            request.SalesId,
            cancellationToken);

        if (sales is null)
        {
            return Result.Failure(
                new Error(
                    "Sales.NotFound",
                    "Sales document was not found.",
                    ErrorType.NotFound));
        }

        if (sales.Status != DocumentStatus.Submitted)
        {
            return Result.Failure(
                new Error(
                    "Sales.InvalidStatus",
                    "Only Submitted sales documents can be approved.",
                    ErrorType.Validation));
        }

        sales.Approve();

        _salesRepository.Update(sales);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
