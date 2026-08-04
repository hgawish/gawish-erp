using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler
    : IRequestHandler<CreateSupplierCommand, Guid>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var existingSupplier =
            await _supplierRepository.GetByCodeAsync(request.Code);

        if (existingSupplier is not null)
        {
            throw new InvalidOperationException(
                $"Supplier code '{request.Code}' already exists.");
        }

        var supplier = new Supplier(
            request.Code,
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

        _supplierRepository.Add(supplier);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}