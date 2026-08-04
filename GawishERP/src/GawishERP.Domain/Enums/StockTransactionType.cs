namespace GawishERP.Domain.Entities;

public enum StockTransactionType
{
    OpeningBalance = 1,

    Purchase = 2,

    // NEW
    PurchaseCancellation = 3,

    Sale = 4,

    TransferIn = 5,

    TransferOut = 6,

    AdjustmentIncrease = 7,

    AdjustmentDecrease = 8,

    PurchaseReturn = 9,

    SalesReturn = 10,

    Production = 11,

    Consumption = 12
}