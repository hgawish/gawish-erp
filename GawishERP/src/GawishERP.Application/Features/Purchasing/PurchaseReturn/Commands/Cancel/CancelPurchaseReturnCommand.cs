using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Cancel;

public sealed record CancelPurchaseReturnCommand(
    Guid PurchaseReturnId)
    : IRequest<CancelPurchaseReturnResponse>;