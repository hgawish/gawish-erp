using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Approve;

public sealed class ApproveOpeningBalanceCommandHandler
    : IRequestHandler<ApproveOpeningBalanceCommand>
{
    private readonly IOpeningBalanceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveOpeningBalanceCommandHandler(
        IOpeningBalanceRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ApproveOpeningBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (entity is null)
            throw new InvalidOperationException(
                "Opening Balance was not found.");

        entity.Approve();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}