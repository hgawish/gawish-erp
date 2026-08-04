using GawishERP.Application.Common.Mapping;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Suppliers.DTOs;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler
    : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSupplierByIdQueryHandler(
        ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<Result<SupplierDto>> Handle(
        GetSupplierByIdQuery request,
        CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id);

        if (supplier is null)
        {
            return Result.Failure<SupplierDto>(
                new Error(
                    "Supplier.NotFound",
                    $"Supplier '{request.Id}' was not found.",
                    ErrorType.NotFound));
        }

        var dto = SupplierMapper.ToDto(supplier);

        return Result.Success(dto);
    }
}