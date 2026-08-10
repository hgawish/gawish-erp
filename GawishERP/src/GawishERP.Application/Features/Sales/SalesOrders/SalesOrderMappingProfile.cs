using AutoMapper;
using GawishERP.Application.Features.Sales.SalesOrders.Dtos;
using GawishERP.Domain.Entities;

namespace GawishERP.Application.Features.Sales.SalesOrders;

public sealed class SalesOrderMappingProfile : Profile
{
    public SalesOrderMappingProfile()
    {
        CreateMap<SalesOrder, SalesOrderDto>()
            .ForMember(
                d => d.CustomerName,
                o => o.MapFrom(s => s.Customer.Name))

            .ForMember(
                d => d.Lines,
                o => o.MapFrom(s => s.Lines));

        CreateMap<SalesOrderLine, SalesOrderLineDto>()
            .ForMember(
                d => d.ProductName,
                o => o.MapFrom(s => s.Product.Name))

            .ForMember(
                d => d.WarehouseName,
                o => o.MapFrom(s => s.Warehouse.Name))

            .ForMember(
                d => d.RemainingQuantity,
                o => o.MapFrom(s => s.RemainingQuantity))

            .ForMember(
                d => d.IsCompleted,
                o => o.MapFrom(s => s.IsCompleted));
    }
}