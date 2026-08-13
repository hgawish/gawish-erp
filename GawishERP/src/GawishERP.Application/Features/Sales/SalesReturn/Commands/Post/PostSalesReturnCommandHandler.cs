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
    private readonly ISalesReturnPostingService _salesReturnPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public PostSalesReturnCommandHandler(
        ISalesReturnRepository salesReturnRepository,
        ISalesRepository salesRepository,
        ISalesReturnPostingService salesReturnPostingService,
        IUnitOfWork unitOfWork)
    {
        _salesReturnRepository = salesReturnRepository;
        _salesRepository = salesRepository;
        _salesReturnPostingService = salesReturnPostingService;
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

        if (salesReturn.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Cancelled Sales Return cannot be posted.");

        if (!salesReturn.Lines.Any())
            throw new InvalidOperationException(
                "Sales Return document has no lines.");

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

        foreach (var line in salesReturn.Lines)
        {
            var salesLine =
                sales.Lines.FirstOrDefault(
                    x => x.Id == line.SalesLineId);

            if (salesLine is null)
                throw new InvalidOperationException(
                    "Original Sales Line not found.");

            if (line.ProductId != salesLine.ProductId)
                throw new InvalidOperationException(
                    "Sales Return product does not match the original Sales Line.");

            var previouslyReturnedQuantity =
                await _salesReturnRepository.GetPreviouslyReturnedQuantityAsync(
                    salesReturn.SalesId,
                    line.SalesLineId,
                    salesReturn.Id,
                    cancellationToken);

            var totalReturnedQuantity =
                previouslyReturnedQuantity + line.Quantity;

            if (totalReturnedQuantity > salesLine.Quantity)
            {
                throw new InvalidOperationException(
                    $"Returned quantity exceeds sold quantity for product {salesLine.ProductId}. " +
                    $"Sold: {salesLine.Quantity}, " +
                    $"Previously returned: {previouslyReturnedQuantity}, " +
                    $"Current return: {line.Quantity}.");
            }
        }

        salesReturn.Post();

        // The posting service owns both inventory and accounting posting.
        await _salesReturnPostingService.PostSalesReturnAsync(
            salesReturn,
            cancellationToken);

        _salesReturnRepository.Update(salesReturn);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new PostSalesReturnResponse
        {
            Id = salesReturn.Id,
            DocumentNumber = salesReturn.DocumentNumber,
            Status = salesReturn.Status.ToString(),
            Message = "Sales Return posted successfully."
        };
    }
}
