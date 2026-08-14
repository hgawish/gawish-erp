using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.Accounting.JournalEntries.Commands.Reverse;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Cancel;

public sealed class CancelSalesReturnHandler
    : IRequestHandler<CancelSalesReturnCommand, CancelSalesReturnResponse>
{
    private readonly ISalesReturnRepository _salesReturnRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSalesReturnHandler(
        ISalesReturnRepository salesReturnRepository,
        IInventoryService inventoryService,
        IStockTransactionRepository stockTransactionRepository,
        IJournalEntryRepository journalEntryRepository,
        IMediator mediator,
        IUnitOfWork unitOfWork)
    {
        _salesReturnRepository = salesReturnRepository;
        _inventoryService = inventoryService;
        _stockTransactionRepository = stockTransactionRepository;
        _journalEntryRepository = journalEntryRepository;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelSalesReturnResponse> Handle(
        CancelSalesReturnCommand request,
        CancellationToken cancellationToken)
    {
        var salesReturn =
            await _salesReturnRepository.GetByIdWithLinesAsync(
                request.SalesReturnId,
                cancellationToken);

        if (salesReturn is null)
            throw new InvalidOperationException(
                "Sales return document not found.");

        if (salesReturn.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Sales return document already cancelled.");

        if (salesReturn.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted sales return documents can be cancelled.");

        await _unitOfWork.BeginTransactionAsync(
            cancellationToken);

        try
        {
            //=========================================================
            // 1. Reverse Stock using the historical UnitCost
            //    recorded by the original Sales Return transaction.
            //=========================================================

            var stockTransactions =
                await _stockTransactionRepository.GetByReferenceAsync(
                    salesReturn.Id,
                    StockTransactionType.SalesReturn);

            foreach (var line in salesReturn.Lines)
            {
                var stockTransaction =
                    stockTransactions
                        .Where(x => x.ProductId == line.ProductId)
                        .OrderByDescending(x => x.TransactionDate)
                        .FirstOrDefault();

                if (stockTransaction is null)
                {
                    throw new InvalidOperationException(
                        $"Original stock transaction not found for product {line.ProductId}.");
                }

                await _inventoryService.ReverseSalesReturnAsync(
                    line.ProductId,
                    salesReturn.WarehouseId,
                    line.Quantity,
                    stockTransaction.UnitCost,
                    salesReturn.DocumentDate,
                    salesReturn.Id,
                    salesReturn.DocumentNumber,
                    salesReturn.Notes,
                    cancellationToken);
            }

            //=========================================================
            // 2. Reverse the original Journal Entry
            //=========================================================

            var journalEntry =
                await _journalEntryRepository.GetPostedByReferenceNumberAsync(
                    salesReturn.DocumentNumber,
                    DocumentType.SalesReturn,
                    cancellationToken);

            if (journalEntry is null)
            {
                throw new InvalidOperationException(
                    $"Posted journal entry not found for sales return {salesReturn.DocumentNumber}.");
            }

            var reverseResult =
                await _mediator.Send(
                    new ReverseJournalEntryCommand(journalEntry.Id),
                    cancellationToken);

            if (reverseResult.IsFailure)
            {
                throw new InvalidOperationException(
                    reverseResult.Error.Message);
            }

            //=========================================================
            // 3. Cancel Sales Return
            //=========================================================

            salesReturn.Cancel();

            _salesReturnRepository.Update(salesReturn);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            //=========================================================
            // 4. Commit everything
            //=========================================================

            await _unitOfWork.CommitTransactionAsync(
                cancellationToken);

            return new CancelSalesReturnResponse
            {
                Id = salesReturn.Id,
                DocumentNumber = salesReturn.DocumentNumber,
                Status = salesReturn.Status.ToString(),
                Message = "Sales return cancelled successfully."
            };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(
                CancellationToken.None);

            throw;
        }
    }
}