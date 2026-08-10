using AutoMapper;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Application.Features.Sales.SalesDeliveries.Dtos;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Queries.GetSalesDeliveries;

public sealed class GetSalesDeliveriesHandler
    : IRequestHandler<GetSalesDeliveriesQuery, List<SalesDeliveryDto>>
{
    private readonly ISalesDeliveryRepository _repository;
    private readonly IMapper _mapper;

    public GetSalesDeliveriesHandler(
        ISalesDeliveryRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<SalesDeliveryDto>> Handle(
        GetSalesDeliveriesQuery request,
        CancellationToken cancellationToken)
    {
        var entities =
            await _repository.GetAllAsync(
                cancellationToken);

        return _mapper.Map<List<SalesDeliveryDto>>(
            entities);
    }
}