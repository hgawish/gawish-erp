using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Interfaces;

public interface IPurchasePostingService
{
    Task PostPurchaseInvoiceAsync(
        PurchaseHeader purchase,
        CancellationToken cancellationToken = default);

    Task ReversePurchaseInvoiceAsync(
        Guid purchaseId,
        CancellationToken cancellationToken = default);
}