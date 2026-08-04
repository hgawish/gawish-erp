using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Cancel;

public sealed record CancelPurchaseCommand(Guid PurchaseId)
    : IRequest<CancelPurchaseResponse>;