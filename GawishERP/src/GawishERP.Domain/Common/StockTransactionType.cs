namespace GawishERP.Domain.Common;

public enum StockTransactionType
{
    OpeningBalance = 1,

    Purchase = 2,

    PurchaseReturn = 3,

    Sale = 4,

    SalesReturn = 5,

    AdjustmentIncrease = 6,

    AdjustmentDecrease = 7,

    TransferIn = 8,

    TransferOut = 9,

    ProductionIn = 10,

    ProductionOut = 11,

    StockCountIncrease = 12,

    StockCountDecrease = 13
}