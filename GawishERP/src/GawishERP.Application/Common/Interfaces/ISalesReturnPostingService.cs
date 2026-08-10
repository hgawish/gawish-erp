using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Interfaces;

public interface ISalesReturnPostingService
{
    Task PostSalesReturnAsync(
        SalesReturnHeader salesReturn,
        CancellationToken cancellationToken = default);

    Task ReverseSalesReturnAsync(
        Guid salesReturnId,
        CancellationToken cancellationToken = default);
}