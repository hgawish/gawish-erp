using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandHandler
    : IRequestHandler<UpdateSupplierCommand, Guid>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        UpdateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id);

        if (supplier is null)
        {
            throw new InvalidOperationException(
                $"Supplier '{request.Id}' was not found.");
        }

        supplier.Update(
            request.Name,
            request.ArabicName,
            request.ContactPerson,
            request.Phone,
            request.Mobile,
            request.Email,
            request.TaxNumber,
            request.CommercialRegistration,
            request.Address,
            request.City,
            request.Country,
            request.Notes,
            request.AccountId);

        if (request.IsActive)
        {
            supplier.Activate();
        }
        else
        {
            supplier.Deactivate();
        }

        _supplierRepository.Update(supplier);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}