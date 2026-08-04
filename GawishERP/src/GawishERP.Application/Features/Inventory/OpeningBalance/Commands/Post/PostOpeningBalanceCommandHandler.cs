using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Commands.Post;

public sealed class PostOpeningBalanceCommandHandler
    : IRequestHandler<PostOpeningBalanceCommand>
{
    private readonly IOpeningBalanceRepository _openingBalanceRepository;
    private readonly IInventoryService _inventoryService;

    public PostOpeningBalanceCommandHandler(
        IOpeningBalanceRepository openingBalanceRepository,
        IInventoryService inventoryService)
    {
        _openingBalanceRepository = openingBalanceRepository;
        _inventoryService = inventoryService;
    }

    public async Task Handle(
        PostOpeningBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var document = await _openingBalanceRepository.GetByIdAsync(request.Id);
     
        if (document is null)
            throw new InvalidOperationException(
                "Opening Balance document not found.");

        // تغيير حالة المستند إلى Posted
        document.Post();

        // إنشاء حركات المخزون
        foreach (var line in document.Lines)
        {
            await _inventoryService.AddOpeningBalanceAsync(
                line.ProductId,
                document.WarehouseId,
                line.Quantity,
                line.UnitCost,
                document.DocumentDate,
                document.DocumentNumber,
                document.Id,
                line.Notes);
        }

        // تحديث حالة المستند
        _openingBalanceRepository.Update(document);

        // حفظ جميع التعديلات مرة واحدة
        await _openingBalanceRepository.SaveChangesAsync(cancellationToken);
    }
}