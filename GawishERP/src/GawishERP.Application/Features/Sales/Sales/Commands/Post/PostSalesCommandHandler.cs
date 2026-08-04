using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Post;

public sealed class PostSalesCommandHandler
    : IRequestHandler<PostSalesCommand, PostSalesResponse>
{
    private readonly ISalesRepository _salesRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public PostSalesCommandHandler(
        ISalesRepository salesRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _salesRepository = salesRepository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PostSalesResponse> Handle(
        PostSalesCommand request,
        CancellationToken cancellationToken)
    {
        var sales =
            await _salesRepository.GetByIdWithLinesAsync(
                request.SalesId,
                cancellationToken);

        if (sales is null)
            throw new InvalidOperationException(
                "Sales document not found.");

        if (sales.Status == DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Sales document already posted.");

        if (sales.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Cancelled sales document cannot be posted.");

        if (!sales.Lines.Any())
            throw new InvalidOperationException(
                "Sales document has no lines.");

        foreach (var line in sales.Lines)
        {
            await _inventoryService.AddSaleAsync(
                line.ProductId,
                sales.WarehouseId,
                line.Quantity,
                line.UnitPrice,
                sales.DocumentDate,
                sales.Id,
                sales.DocumentNumber,
                sales.Notes,
                cancellationToken);
        }

        sales.Post();

        _salesRepository.Update(sales);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PostSalesResponse
        {
            Id = sales.Id,
            DocumentNumber = sales.DocumentNumber,
            Status = sales.Status.ToString(),
            Message = "Sales posted successfully."
        };
    }
}