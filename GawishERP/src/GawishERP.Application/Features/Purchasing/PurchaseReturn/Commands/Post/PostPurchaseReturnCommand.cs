using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Post;

public sealed record PostPurchaseReturnCommand(
    Guid PurchaseReturnId)
    : IRequest<PostPurchaseReturnResponse>;