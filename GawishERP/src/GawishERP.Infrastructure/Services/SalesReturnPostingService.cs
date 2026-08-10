using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public sealed class SalesReturnPostingService : ISalesReturnPostingService
{
    private readonly IPostingEngine _postingEngine;
    private readonly ISalesReturnRepository _salesReturnRepository;

    public SalesReturnPostingService(
        IPostingEngine postingEngine,
        ISalesReturnRepository salesReturnRepository)
    {
        _postingEngine = postingEngine;
        _salesReturnRepository = salesReturnRepository;
    }

    public async Task PostSalesReturnAsync(
        SalesReturnHeader salesReturn,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(salesReturn);

        var context = new PostingContext
        {
            DocumentId = salesReturn.Id,
            DocumentType = DocumentType.SalesReturn,
            DocumentNumber = salesReturn.DocumentNumber,
            PostingDate = salesReturn.DocumentDate,
            FiscalYearId = salesReturn.FiscalYearId,
            CompanyId = salesReturn.CompanyId,
            BranchId = salesReturn.BranchId,
            ReferenceNumber = salesReturn.DocumentNumber,
            Description = salesReturn.Notes,
            Amount = salesReturn.TotalAmount,

            Lines = salesReturn.Lines
                .Select(x => new PostingLineContext
                {
                    ProductId = x.ProductId,
                    WarehouseId = salesReturn.WarehouseId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,

                    // التصحيح
                    BatchNumber = x.SalesLine.BatchNumber,
                    ExpiryDate = x.SalesLine.ExpiryDate,

                    Description = x.Notes
                })
                .ToList()
        };

        await _postingEngine.PostDocumentAsync(
            context,
            cancellationToken);
    }

    public async Task ReverseSalesReturnAsync(
        Guid salesReturnId,
        CancellationToken cancellationToken = default)
    {
        var salesReturn =
            await _salesReturnRepository.GetByIdWithLinesAsync(
                salesReturnId,
                cancellationToken);

        if (salesReturn is null)
            throw new InvalidOperationException(
                "Sales Return not found.");

        var context = new PostingContext
        {
            DocumentId = salesReturn.Id,
            DocumentType = DocumentType.SalesReturn,
            DocumentNumber = salesReturn.DocumentNumber,
            PostingDate = salesReturn.DocumentDate,
            FiscalYearId = salesReturn.FiscalYearId,
            CompanyId = salesReturn.CompanyId,
            BranchId = salesReturn.BranchId,
            ReferenceNumber = salesReturn.DocumentNumber,
            Description = salesReturn.Notes,
            Amount = salesReturn.TotalAmount,

            Lines = salesReturn.Lines
                .Select(x => new PostingLineContext
                {
                    ProductId = x.ProductId,
                    WarehouseId = salesReturn.WarehouseId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,

                    // التصحيح
                    BatchNumber = x.SalesLine.BatchNumber,
                    ExpiryDate = x.SalesLine.ExpiryDate,

                    Description = x.Notes
                })
                .ToList()
        };

        await _postingEngine.ReverseDocumentAsync(
            context,
            cancellationToken);
    }
}