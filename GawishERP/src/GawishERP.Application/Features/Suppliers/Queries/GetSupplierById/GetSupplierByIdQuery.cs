using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Suppliers.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery(Guid Id)
    : IRequest<Result<SupplierDto>>;