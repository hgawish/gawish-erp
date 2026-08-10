using AutoMapper;
using GawishERP.Application.Features.Sales.SalesDeliveries.Dtos;
using GawishERP.Domain.Entities;

namespace GawishERP.Application.Features.Sales.SalesDeliveries;

public sealed class SalesDeliveryMappingProfile : Profile
{
    public SalesDeliveryMappingProfile()
    {
        CreateMap<SalesDelivery, SalesDeliveryDto>()
            .ForMember(
                d => d.CustomerName,
                o => o.MapFrom(s => s.Customer.Name));

        CreateMap<SalesDeliveryLine, SalesDeliveryLineDto>()
            .ForMember(
                d => d.ProductName,
                o => o.MapFrom(s => s.Product.Name))

            .ForMember(
                d => d.WarehouseName,
                o => o.MapFrom(s => s.Warehouse.Name));
    }
}