using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Queries.GetById;

public sealed class GetSalesReturnByIdHandler
    : IRequestHandler<GetSalesReturnByIdQuery, GetSalesReturnByIdResponse>
{
    private readonly ISalesReturnRepository _salesReturnRepository;

    public GetSalesReturnByIdHandler(
        ISalesReturnRepository salesReturnRepository)
    {
        _salesReturnRepository = salesReturnRepository;
    }

    public async Task<GetSalesReturnByIdResponse> Handle(
        GetSalesReturnByIdQuery request,
        CancellationToken cancellationToken)
    {
        var salesReturn = await _salesReturnRepository.GetByIdForViewAsync(
            request.Id,
            cancellationToken);

        if (salesReturn is null)
            throw new KeyNotFoundException("Sales Return not found.");

        return new GetSalesReturnByIdResponse
        {
            Id = salesReturn.Id,

            DocumentNumber = salesReturn.DocumentNumber,

            DocumentDate = salesReturn.DocumentDate,

            Status = salesReturn.Status.ToString(),

            SalesId = salesReturn.SalesId,

            CustomerId = salesReturn.CustomerId,

            CustomerName = salesReturn.Customer.Name,

            WarehouseId = salesReturn.WarehouseId,

            WarehouseName = salesReturn.Warehouse.Name,

            ReturnReason = salesReturn.ReturnReason,

            TotalAmount = salesReturn.TotalAmount,

            Notes = salesReturn.Notes,

            Lines = salesReturn.Lines
                .Select(line => new SalesReturnLineDto
                {
                    Id = line.Id,

                    ProductId = line.ProductId,

                    ProductCode = line.Product.Code,

                    ProductName = line.Product.Name,

                    Quantity = line.Quantity,

                    UnitPrice = line.UnitPrice,

                    LineTotal = line.LineTotal,

                    Notes = line.Notes
                })
                .ToList()
        };
    }
}