using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Infrastructure.Persistence.Seed;

public static class NumberSeriesSeeder
{
    public static IEnumerable<NumberSeries> GetDefaultSeries()
    {
        return new List<NumberSeries>
        {
            new(DocumentType.OpeningBalance, "OB"),
            new(DocumentType.Purchase, "PO"),
            new(DocumentType.PurchaseReturn, "PR"),
            new(DocumentType.Sales, "SO"),
            new(DocumentType.SalesReturn, "SR"),
            new(DocumentType.Transfer, "TR"),
            new(DocumentType.Adjustment, "ADJ"),
            new(DocumentType.Production, "MO"),
            new(DocumentType.StockCount, "SC")
        };
    }
}