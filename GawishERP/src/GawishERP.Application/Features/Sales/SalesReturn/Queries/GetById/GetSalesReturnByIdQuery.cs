using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Queries.GetById;

public sealed record GetSalesReturnByIdQuery(Guid Id)
    : IRequest<GetSalesReturnByIdResponse>;