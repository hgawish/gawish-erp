namespace GawishERP.Application.Features.Sales.Sales.Queries.GetList;

public sealed class GetSalesListResponse
{
    public List<SalesListItemDto> Items { get; init; } = new();

    public int TotalCount { get; init; }

    public int PageNumber { get; init; }

    public int PageSize { get; init; }
}