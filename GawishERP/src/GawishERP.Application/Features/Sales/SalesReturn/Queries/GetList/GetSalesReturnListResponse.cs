namespace GawishERP.Application.Features.Sales.SalesReturn.Queries.GetList;

public sealed class GetSalesReturnListResponse
{
    public List<SalesReturnListItemDto> Items { get; set; } = new();

    public int TotalCount { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}