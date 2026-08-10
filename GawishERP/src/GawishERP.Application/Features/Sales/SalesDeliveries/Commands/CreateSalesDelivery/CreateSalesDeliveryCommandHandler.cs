using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.CreateSalesDelivery;

public sealed class CreateSalesDeliveryCommandHandler
    : IRequestHandler<CreateSalesDeliveryCommand, Guid>
{
    private readonly ISalesDeliveryRepository _repository;
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSalesDeliveryCommandHandler(
        ISalesDeliveryRepository repository,
        ISalesOrderRepository salesOrderRepository,
        IDocumentNumberService documentNumberService,
        IFiscalYearRepository fiscalYearRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _salesOrderRepository = salesOrderRepository;
        _documentNumberService = documentNumberService;
        _fiscalYearRepository = fiscalYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateSalesDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        //====================================================
        // Basic Validation
        //====================================================

        if (request.SalesOrderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Sales Order ID cannot be empty.",
                nameof(request.SalesOrderId));
        }

        if (request.Lines is null || request.Lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Sales Delivery must contain at least one line.");
        }

        //====================================================
        // Get Sales Order
        //====================================================

        var salesOrder =
            await _salesOrderRepository.GetByIdAsync(
                request.SalesOrderId,
                cancellationToken);

        if (salesOrder is null)
        {
            throw new InvalidOperationException(
                "Sales Order was not found.");
        }

        //====================================================
        // Sales Order must be Approved
        //====================================================

        if (salesOrder.Status != DocumentStatus.Approved)
        {
            throw new InvalidOperationException(
                "Sales Delivery can only be created from an approved Sales Order.");
        }

        //====================================================
        // Get Open Fiscal Year
        //====================================================

        var fiscalYear =
            await _fiscalYearRepository.GetOpenFiscalYearAsync();

        if (fiscalYear is null)
        {
            throw new InvalidOperationException(
                "No open fiscal year was found.");
        }

        //====================================================
        // Validate all requested lines BEFORE changing anything
        //====================================================

        foreach (var requestLine in request.Lines)
        {
            if (requestLine.SalesOrderLineId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Sales Order Line ID cannot be empty.",
                    nameof(requestLine.SalesOrderLineId));
            }

            if (requestLine.Quantity <= 0)
            {
                throw new ArgumentException(
                    "Delivery quantity must be greater than zero.",
                    nameof(requestLine.Quantity));
            }

            var orderLine =
                salesOrder.Lines.FirstOrDefault(
                    x => x.Id == requestLine.SalesOrderLineId);

            if (orderLine is null)
            {
                throw new InvalidOperationException(
                    $"Sales Order Line '{requestLine.SalesOrderLineId}' was not found.");
            }

            //================================================
            // Prevent Over Delivery
            //================================================

            if (requestLine.Quantity > orderLine.RemainingQuantity)
            {
                throw new InvalidOperationException(
                    $"Delivery quantity ({requestLine.Quantity}) " +
                    $"exceeds remaining quantity ({orderLine.RemainingQuantity}) " +
                    $"for Sales Order Line '{requestLine.SalesOrderLineId}'.");
            }
        }

        //====================================================
        // Generate Document Number
        //====================================================

        var documentNumber =
            await _documentNumberService.GenerateAsync(
                DocumentType.SalesDelivery,
                salesOrder.CompanyId,
                salesOrder.BranchId,
                fiscalYear.Id,
                cancellationToken);

        //====================================================
        // Create Delivery
        //====================================================

        var delivery = new SalesDelivery(
            documentNumber,
            request.DocumentDate,
            fiscalYear.Id,
            salesOrder.Id,
            salesOrder.CustomerId,
            salesOrder.CompanyId,
            salesOrder.BranchId,
            request.Notes);

        //====================================================
        // Add Delivery Lines
        // AND Update Sales Order Delivered Quantity
        //====================================================

        foreach (var requestLine in request.Lines)
        {
            var orderLine =
                salesOrder.Lines.First(
                    x => x.Id == requestLine.SalesOrderLineId);

            //================================================
            // Add Sales Delivery Line
            //================================================

            delivery.AddLine(
                orderLine.Id,
                orderLine.ProductId,
                orderLine.WarehouseId,
                requestLine.Quantity);

            //================================================
            // Update Sales Order Line
            //================================================

            orderLine.Deliver(
                requestLine.Quantity);
        }

        //====================================================
        // Add Sales Delivery
        //====================================================

        await _repository.AddAsync(
            delivery,
            cancellationToken);

        //====================================================
        // Save Everything in Same Unit of Work
        //====================================================

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return delivery.Id;
    }
}