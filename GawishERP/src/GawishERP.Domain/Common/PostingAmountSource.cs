namespace GawishERP.Domain.Common;

public enum PostingAmountSource
{
    NetTotal = 1,

    TotalBeforeDiscount = 2,

    Discount = 3,

    Tax = 4,

    Cost = 5,

    Quantity = 6,

    Custom = 100
}