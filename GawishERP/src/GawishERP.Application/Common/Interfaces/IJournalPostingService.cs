using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Interfaces;

public interface IJournalPostingService
{
    Task PostSalesAsync(
        SalesHeader sales,
        CancellationToken cancellationToken = default);

    Task PostSalesReturnAsync(
        SalesReturnHeader salesReturn,
        CancellationToken cancellationToken = default);

    Task PostPurchaseAsync(
        PurchaseHeader purchase,
        CancellationToken cancellationToken = default);

    Task PostPurchaseReturnAsync(
        PurchaseReturnHeader purchaseReturn,
        CancellationToken cancellationToken = default);
}