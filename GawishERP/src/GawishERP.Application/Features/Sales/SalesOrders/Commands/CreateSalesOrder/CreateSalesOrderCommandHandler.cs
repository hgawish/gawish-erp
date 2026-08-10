using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.CreateSalesOrder;

public sealed class CreateSalesOrderCommandHandler
    : IRequestHandler<CreateSalesOrderCommand, Guid>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSalesOrderCommandHandler(
        ISalesOrderRepository repository,
        IDocumentNumberService documentNumberService,
        IFiscalYearRepository fiscalYearRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _documentNumberService = documentNumberService;
        _fiscalYearRepository = fiscalYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateSalesOrderCommand request,
        CancellationToken cancellationToken)
    {
        //====================================================
        // Get current open fiscal year
        //====================================================

        var fiscalYear =
            await _fiscalYearRepository.GetOpenFiscalYearAsync();

        if (fiscalYear is null)
        {
            throw new InvalidOperationException(
                "No open fiscal year was found.");
        }

        //====================================================
        // Generate Sales Order Number
        //====================================================

        var documentNumber =
            await _documentNumberService.GenerateAsync(
                DocumentType.Sales,
                cancellationToken);

        //====================================================
        // Create Sales Order
        //====================================================

        var order = new SalesOrder(
            documentNumber,
            request.DocumentDate,
            fiscalYear.Id,
            request.CustomerId,
            request.SalesQuotationId,
            null,
            null,
            request.Notes);

        //====================================================
        // Add Lines
        //====================================================

        foreach (var line in request.Lines)
        {
            order.AddLine(
                line.ProductId,
                line.WarehouseId,
                line.Quantity,
                line.UnitPrice,
                line.DiscountPercent,
                line.TaxPercent);
        }

        //====================================================
        // Save
        //====================================================

        await _repository.AddAsync(
            order,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return order.Id;
    }
}