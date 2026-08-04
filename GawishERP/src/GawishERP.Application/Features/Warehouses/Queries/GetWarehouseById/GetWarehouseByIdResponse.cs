namespace GawishERP.Application.Features.Warehouses.Queries.GetWarehouseById;

public class GetWarehouseByIdResponse
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? ArabicName { get; set; }

    public string? Manager { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }
}