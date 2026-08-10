using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Interfaces;

public interface IPurchaseReturnPostingService
{
    Task PostPurchaseReturnAsync(
        PurchaseReturnHeader purchaseReturn,
        CancellationToken cancellationToken = default);

    Task ReversePurchaseReturnAsync(
        Guid purchaseReturnId,
        CancellationToken cancellationToken = default);
}