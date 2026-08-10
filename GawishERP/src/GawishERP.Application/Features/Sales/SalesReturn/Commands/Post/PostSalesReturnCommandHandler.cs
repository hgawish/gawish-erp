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
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public PostSalesReturnCommandHandler(
        ISalesReturnRepository salesReturnRepository,
        ISalesRepository salesRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _salesReturnRepository = salesReturnRepository;
        _salesRepository = salesRepository;
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
        // Validate Return Lines
        //=========================================================

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
        }

        //=========================================================
        // Inventory
        //=========================================================

        foreach (var line in salesReturn.Lines)
        {
            await _inventoryService.ReverseSaleAsync(
                line.ProductId,
                salesReturn.WarehouseId,
                line.Quantity,
                line.UnitPrice,
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