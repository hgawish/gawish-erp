using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Post;

public sealed class PostSalesCommandHandler
    : IRequestHandler<PostSalesCommand, PostSalesResponse>
{
    private readonly ISalesRepository _salesRepository;
    private readonly ISalesPostingService _salesPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public PostSalesCommandHandler(
        ISalesRepository salesRepository,
        ISalesPostingService salesPostingService,
        IUnitOfWork unitOfWork)
    {
        _salesRepository = salesRepository;
        _salesPostingService = salesPostingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PostSalesResponse> Handle(
        PostSalesCommand request,
        CancellationToken cancellationToken)
    {
        //=========================================================
        // Load Sales
        //=========================================================

        var sales =
            await _salesRepository.GetByIdWithLinesAsync(
                request.SalesId,
                cancellationToken);

        if (sales is null)
        {
            throw new InvalidOperationException(
                "Sales document not found.");
        }

        //=========================================================
        // Status Validation
        //=========================================================

        if (sales.Status == DocumentStatus.Posted)
        {
            throw new InvalidOperationException(
                "Sales document already posted.");
        }

        if (sales.Status == DocumentStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Cancelled sales document cannot be posted.");
        }

        //=========================================================
        // Lines Validation
        //=========================================================

        if (!sales.Lines.Any())
        {
            throw new InvalidOperationException(
                "Sales document has no lines.");
        }

        //=========================================================
        // Posting
        //
        // SalesPostingService is responsible for:
        //
        // 1. Inventory decrease
        // 2. Actual inventory costing
        // 3. COGS calculation
        // 4. Journal Entry creation
        //
        // DO NOT call InventoryService.AddSaleAsync here.
        //=========================================================

        await _salesPostingService.PostSalesInvoiceAsync(
            sales,
            cancellationToken);

        //=========================================================
        // Post Sales Document
        //=========================================================

        sales.Post();

        //=========================================================
        // Save Sales Status
        //=========================================================

        _salesRepository.Update(sales);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        //=========================================================
        // Response
        //=========================================================

        return new PostSalesResponse
        {
            Id = sales.Id,
            DocumentNumber = sales.DocumentNumber,
            Status = sales.Status.ToString(),
            Message = "Sales posted successfully."
        };
    }
}