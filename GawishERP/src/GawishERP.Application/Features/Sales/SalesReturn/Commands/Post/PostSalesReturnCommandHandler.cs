using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Post;

public sealed class PostSalesReturnCommandHandler
    : IRequestHandler<PostSalesReturnCommand, PostSalesReturnResponse>
{
    private readonly ISalesReturnRepository _salesReturnRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public PostSalesReturnCommandHandler(
        ISalesReturnRepository salesReturnRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _salesReturnRepository = salesReturnRepository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PostSalesReturnResponse> Handle(
        PostSalesReturnCommand request,
        CancellationToken cancellationToken)
    {
        var salesReturn =
            await _salesReturnRepository.GetByIdWithLinesAsync(
                request.SalesReturnId,
                cancellationToken);

        if (salesReturn is null)
            throw new InvalidOperationException(
                "Sales Return document not found.");

        if (salesReturn.Status == DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Sales Return already posted.");

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

        salesReturn.Post();

        _salesReturnRepository.Update(salesReturn);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PostSalesReturnResponse
        {
            Id = salesReturn.Id,
            DocumentNumber = salesReturn.DocumentNumber,
            Status = salesReturn.Status.ToString(),
            Message = "Sales Return posted successfully."
        };
    }
}