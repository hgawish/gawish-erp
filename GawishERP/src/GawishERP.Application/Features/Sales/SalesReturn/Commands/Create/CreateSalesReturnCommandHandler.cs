using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Create;

public sealed class CreateSalesReturnCommandHandler
    : IRequestHandler<CreateSalesReturnCommand, CreateSalesReturnResponse>
{
    private readonly ISalesRepository _salesRepository;
    private readonly ISalesReturnRepository _salesReturnRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductRepository _productRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSalesReturnCommandHandler(
        ISalesRepository salesRepository,
        ISalesReturnRepository salesReturnRepository,
        ICustomerRepository customerRepository,
        IWarehouseRepository warehouseRepository,
        IProductRepository productRepository,
        IDocumentNumberService documentNumberService,
        IUnitOfWork unitOfWork)
    {
        _salesRepository = salesRepository;
        _salesReturnRepository = salesReturnRepository;
        _customerRepository = customerRepository;
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
        _documentNumberService = documentNumberService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateSalesReturnResponse> Handle(
        CreateSalesReturnCommand request,
        CancellationToken cancellationToken)
    {
        var sales =
            await _salesRepository.GetByIdWithLinesAsync(
                request.SalesId,
                cancellationToken);

        if (sales is null)
            throw new InvalidOperationException("Sales document not found.");

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
                DocumentType.SalesReturn,
                cancellationToken);

        var salesReturn = new SalesReturnHeader(
            documentNumber,
            request.DocumentDate,

            request.FiscalYearId,
            request.CompanyId,
            request.BranchId,

            request.SalesId,
            request.CustomerId,
            request.WarehouseId,
            request.ReturnReason,
            request.Notes);

        foreach (var line in request.Lines)
        {
            var product =
                await _productRepository.GetByIdAsync(line.ProductId);

            if (product is null)
                throw new InvalidOperationException(
                    $"Product ({line.ProductId}) not found.");

            salesReturn.AddLine(
                line.SalesLineId,
                line.ProductId,
                line.Quantity,
                line.UnitPrice,
                line.Notes);
        }

        _salesReturnRepository.Add(salesReturn);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateSalesReturnResponse
        {
            Id = salesReturn.Id,
            DocumentNumber = salesReturn.DocumentNumber,
            Status = salesReturn.Status.ToString()
        };
    }
}