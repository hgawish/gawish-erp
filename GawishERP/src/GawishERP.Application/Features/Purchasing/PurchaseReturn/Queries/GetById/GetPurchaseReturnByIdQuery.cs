using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Queries.GetById;

public sealed record GetPurchaseReturnByIdQuery(
    Guid Id)
    : IRequest<PurchaseReturnDetailsDto?>;