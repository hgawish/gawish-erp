using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Post;

public sealed record PostPurchaseCommand(
    Guid PurchaseId)
    : IRequest<PostPurchaseResponse>;