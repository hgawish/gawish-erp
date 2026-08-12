using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Post;

public sealed class PostSalesReturnCommandHandler
    : IRequestHandler<PostSalesReturnCommand, PostSalesReturnResponse>
{
    private readonly ISalesReturnRepository _salesReturnRepository;
    private readonly ISalesRepository _salesRepository;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public PostSalesReturnCommandHandler(
        ISalesReturnRepository salesReturnRepository,
        ISalesRepository salesRepository,
        IStockTransactionRepository stockTransactionRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _salesReturnRepository = salesReturnRepository;
        _salesRepository = salesRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PostSalesReturnResponse> Handle(
        PostSalesReturnCommand request,
        CancellationToken cancellationToken)
    {
        //=========================================================
        // Load Sales Return
        //=========================================================

        var salesReturn =
            await _salesReturnRepository.GetByIdWithLinesAsync(
                request.SalesReturnId,
                cancellationToken);

        if (salesReturn is null)
            throw new InvalidOperationException(
                "Sales Return document not found.");

        //=========================================================
        // Status Validation
        //=========================================================

        if (salesReturn.Status == DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Sales Return already posted.");

        if (salesReturn.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Cancelled Sales Return cannot be posted.");

        //=========================================================
        // Validate Lines
        //=========================================================

        if (!salesReturn.Lines.Any())
            throw new InvalidOperationException(
                "Sales Return document has no lines.");

        //=========================================================
        // Load Original Sales
        //=========================================================

        var sales =
            await _salesRepository.GetByIdWithLinesAsync(
                salesReturn.SalesId,
                cancellationToken);

        if (sales is null)
            throw new InvalidOperationException(
                "Original Sales document not found.");

        if (sales.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Original Sales document must be posted.");

        //=========================================================
        // Load Historical Inventory Cost From Original Sale
        //=========================================================

        var saleStockTransactions =
            await _stockTransactionRepository.GetByReferenceAsync(
                sales.Id,
                StockTransactionType.Sale);

        //=========================================================
        // Validate Return Lines And Resolve Historical Cost
        //=========================================================

        var historicalCosts = new Dictionary<Guid, decimal>();

        foreach (var line in salesReturn.Lines)
        {
            //=====================================================
            // Find Original Sales Line
            //=====================================================

            var salesLine =
                sales.Lines.FirstOrDefault(
                    x => x.Id == line.SalesLineId);

            if (salesLine is null)
                throw new InvalidOperationException(
                    "Original Sales Line not found.");

            //=====================================================
            // Product Validation
            //=====================================================

            if (line.ProductId != salesLine.ProductId)
                throw new InvalidOperationException(
                    "Sales Return product does not match the original Sales Line.");

            //=====================================================
            // Previously Posted Returns
            //=====================================================

            var previouslyReturnedQuantity =
                await _salesReturnRepository
                    .GetPreviouslyReturnedQuantityAsync(
                        salesReturn.SalesId,
                        line.SalesLineId,
                        salesReturn.Id,
                        cancellationToken);

            //=====================================================
            // Total Returned Quantity
            //=====================================================

            var totalReturnedQuantity =
                previouslyReturnedQuantity + line.Quantity;

            //=====================================================
            // Prevent Exceeding Original Sale Quantity
            //=====================================================

            if (totalReturnedQuantity > salesLine.Quantity)
            {
                throw new InvalidOperationException(
                    $"Returned quantity exceeds sold quantity for product {salesLine.ProductId}. " +
                    $"Sold: {salesLine.Quantity}, " +
                    $"Previously returned: {previouslyReturnedQuantity}, " +
                    $"Current return: {line.Quantity}.");
            }

            //=====================================================
            // Resolve Historical Cost
            //=====================================================

            var matchingTransactions =
                saleStockTransactions
                    .Where(x =>
                        x.ProductId == line.ProductId &&
                        x.WarehouseId == salesReturn.WarehouseId)
                    .ToList();

            if (matchingTransactions.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Historical inventory cost could not be found for product {line.ProductId} " +
                    $"in warehouse {salesReturn.WarehouseId}.");
            }

            var distinctCosts =
                matchingTransactions
                    .Select(x => x.UnitCost)
                    .Distinct()
                    .ToList();

            if (distinctCosts.Count > 1)
            {
                throw new InvalidOperationException(
                    $"Multiple historical inventory costs were found for product {line.ProductId} " +
                    "on the original Sales document. The return cost cannot be determined safely.");
            }

            historicalCosts[line.Id] = distinctCosts[0];
        }

        //=========================================================
        // Inventory
        //=========================================================

        foreach (var line in salesReturn.Lines)
        {
            var historicalUnitCost =
                historicalCosts[line.Id];

            await _inventoryService.ReverseSaleAsync(
                line.ProductId,
                salesReturn.WarehouseId,
                line.Quantity,
                historicalUnitCost,
                salesReturn.DocumentDate,
                salesReturn.Id,
                salesReturn.DocumentNumber,
                salesReturn.Notes,
                cancellationToken);
        }

        //=========================================================
        // Post Document
        //=========================================================

        salesReturn.Post();

        //=========================================================
        // Save
        //=========================================================

        _salesReturnRepository.Update(salesReturn);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        //=========================================================
        // Response
        //=========================================================

        return new PostSalesReturnResponse
        {
            Id = salesReturn.Id,
            DocumentNumber = salesReturn.DocumentNumber,
            Status = salesReturn.Status.ToString(),
            Message = "Sales Return posted successfully."
        };
    }
}
