using GawishERP.Application.Common.Mapping;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Suppliers.DTOs;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Suppliers.Queries.GetAllSuppliers;

public class GetAllSuppliersQueryHandler
    : IRequestHandler<GetAllSuppliersQuery, PagedResult<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetAllSuppliersQueryHandler(
        ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<PagedResult<SupplierDto>> Handle(
        GetAllSuppliersQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) =
            await _supplierRepository.GetAllAsync(
                request.Search,
                request.IsActive,
                request.SortBy,
                request.Descending,
                request.PageNumber,
                request.PageSize);

        var dto = SupplierMapper.ToDtoList(items);

        return PagedResult<SupplierDto>.Create(
            dto,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}