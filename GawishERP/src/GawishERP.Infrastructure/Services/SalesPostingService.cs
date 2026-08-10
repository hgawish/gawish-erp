using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public sealed class SalesPostingService : ISalesPostingService
{
    private readonly IPostingEngine _postingEngine;
    private readonly ISalesRepository _salesRepository;

    public SalesPostingService(
        IPostingEngine postingEngine,
        ISalesRepository salesRepository)
    {
        _postingEngine = postingEngine;
        _salesRepository = salesRepository;
    }

    public async Task PostSalesInvoiceAsync(
        SalesHeader sales,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sales);

        var context = new PostingContext
        {
            DocumentId = sales.Id,
            DocumentType = DocumentType.Sales,
            DocumentNumber = sales.DocumentNumber,
            PostingDate = sales.DocumentDate,
            FiscalYearId = sales.FiscalYearId,
            CompanyId = sales.CompanyId,
            BranchId = sales.BranchId,

            // تم التصحيح
            ReferenceNumber = sales.DocumentNumber,

            Description = sales.Notes,
            Amount = sales.NetTotal,

            Lines = sales.Lines
                .Select(x => new PostingLineContext
                {
                    ProductId = x.ProductId,
                    WarehouseId = sales.WarehouseId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    BatchNumber = x.BatchNumber,
                    ExpiryDate = x.ExpiryDate,
                    Description = x.Notes
                })
                .ToList()
        };

        await _postingEngine.PostDocumentAsync(
            context,
            cancellationToken);
    }

    public async Task ReverseSalesInvoiceAsync(
        Guid salesId,
        CancellationToken cancellationToken = default)
    {
        var sales =
            await _salesRepository.GetByIdWithLinesAsync(
                salesId,
                cancellationToken);

        if (sales is null)
            throw new InvalidOperationException(
                "Sales Invoice not found.");

        var context = new PostingContext
        {
            DocumentId = sales.Id,
            DocumentType = DocumentType.Sales,
            DocumentNumber = sales.DocumentNumber,
            PostingDate = sales.DocumentDate,
            FiscalYearId = sales.FiscalYearId,
            CompanyId = sales.CompanyId,
            BranchId = sales.BranchId,

            // تم التصحيح
            ReferenceNumber = sales.DocumentNumber,

            Description = sales.Notes,
            Amount = sales.NetTotal,

            Lines = sales.Lines
                .Select(x => new PostingLineContext
                {
                    ProductId = x.ProductId,
                    WarehouseId = sales.WarehouseId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    BatchNumber = x.BatchNumber,
                    ExpiryDate = x.ExpiryDate,
                    Description = x.Notes
                })
                .ToList()
        };

        await _postingEngine.ReverseDocumentAsync(
            context,
            cancellationToken);
    }
}