using MediatR;

using GawishERP.Application.Features.Purchasing.Purchase.Queries.GetById;
public sealed record GetPurchaseByIdQuery(
    Guid Id)
    : IRequest<PurchaseDetailsDto?>;