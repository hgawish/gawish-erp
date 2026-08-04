using MediatR;

namespace GawishERP.Application.Features.Warehouses.Commands.UpdateWarehouse;

public class UpdateWarehouseCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ArabicName { get; set; }

    public string? Manager { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Notes { get; set; }
}