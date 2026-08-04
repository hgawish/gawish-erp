using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Create;

public sealed class CreateSalesCommandHandler
    : IRequestHandler<CreateSalesCommand, CreateSalesResponse>
{
    private readonly ISalesRepository _salesRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductRepository _productRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSalesCommandHandler(
        ISalesRepository salesRepository,
        ICustomerRepository customerRepository,
        IWarehouseRepository warehouseRepository,
        IProductRepository productRepository,
        IDocumentNumberService documentNumberService,
        IUnitOfWork unitOfWork)
    {
        _salesRepository = salesRepository;
        _customerRepository = customerRepository;
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
        _documentNumberService = documentNumberService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateSalesResponse> Handle(
        CreateSalesCommand request,
        CancellationToken cancellationToken)
    {
        var customer =
            await _customerRepository.GetByIdAsync(request.CustomerId);

        if (customer is null)
            throw new InvalidOperationException("Customer not found.");

        var warehouse =
            await _warehouseRepository.GetByIdAsync(request.WarehouseId);

        if (warehouse is null)
            throw new InvalidOperationException("Warehouse not found.");

        var documentNumber =
            await _documentNumberService.GenerateAsync(
                DocumentType.Sales,
                cancellationToken);

        var sales = new SalesHeader(
            documentNumber,
            request.DocumentDate,
            request.CustomerId,
            request.WarehouseId,
            request.Currency,
            request.ExchangeRate,
            request.Notes);

        foreach (var line in request.Lines)
        {
            var product =
                await _productRepository.GetByIdAsync(line.ProductId);

            if (product is null)
                throw new InvalidOperationException(
                    $"Product ({line.ProductId}) not found.");

            sales.AddLine(
                line.ProductId,
                line.Quantity,
                line.UnitPrice,
                line.DiscountAmount,
                line.TaxAmount,
                line.BatchNumber,
                line.ExpiryDate,
                line.Notes);
        }

        _salesRepository.Add(sales);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateSalesResponse
        {
            Id = sales.Id,
            DocumentNumber = sales.DocumentNumber,
            Status = sales.Status.ToString()
        };
    }
}