using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Interfaces;

public interface ISalesPostingService
{
    Task PostSalesInvoiceAsync(
        SalesHeader sales,
        CancellationToken cancellationToken = default);

    Task ReverseSalesInvoiceAsync(
        Guid salesId,
        CancellationToken cancellationToken = default);
}