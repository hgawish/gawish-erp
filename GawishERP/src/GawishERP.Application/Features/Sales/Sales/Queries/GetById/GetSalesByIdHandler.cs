using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Queries.GetById;

public sealed class GetSalesByIdHandler
    : IRequestHandler<GetSalesByIdQuery, GetSalesByIdResponse>
{
    private readonly ISalesRepository _salesRepository;

    public GetSalesByIdHandler(
        ISalesRepository salesRepository)
    {
        _salesRepository = salesRepository;
    }

    public async Task<GetSalesByIdResponse> Handle(
        GetSalesByIdQuery request,
        CancellationToken cancellationToken)
    {
        var sales = await _salesRepository.GetByIdForViewAsync(
            request.SalesId,
            cancellationToken);

        if (sales is null)
            throw new InvalidOperationException("Sales document not found.");

        return new GetSalesByIdResponse
        {
            Id = sales.Id,
            DocumentNumber = sales.DocumentNumber,
            DocumentDate = sales.DocumentDate,
            CustomerId = sales.CustomerId,
            CustomerName = sales.Customer.Name,
            WarehouseId = sales.WarehouseId,
            WarehouseName = sales.Warehouse.Name,
            Currency = sales.Currency,
            ExchangeRate = sales.ExchangeRate,
            TotalBeforeDiscount = sales.TotalBeforeDiscount,
            DiscountAmount = sales.DiscountAmount,
            TaxAmount = sales.TaxAmount,
            NetTotal = sales.NetTotal,
            Status = sales.Status.ToString(),
            Notes = sales.Notes,

            Lines = sales.Lines.Select(x => new GetSalesLineDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductCode = x.Product.Code,
                ProductName = x.Product.Name,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                DiscountAmount = x.DiscountAmount,
                TaxAmount = x.TaxAmount,
                LineTotal = x.LineTotal,
                BatchNumber = x.BatchNumber,
                ExpiryDate = x.ExpiryDate,
                Notes = x.Notes
            }).ToList()
        };
    }
}