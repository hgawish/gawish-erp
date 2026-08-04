using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Queries.GetById;

public sealed record GetSalesByIdQuery(Guid SalesId)
    : IRequest<GetSalesByIdResponse>;