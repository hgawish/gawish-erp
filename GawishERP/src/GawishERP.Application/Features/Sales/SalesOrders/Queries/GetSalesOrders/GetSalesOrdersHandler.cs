using AutoMapper;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Application.Features.Sales.SalesOrders.Dtos;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Queries.GetSalesOrders;

public sealed class GetSalesOrdersHandler
    : IRequestHandler<GetSalesOrdersQuery, List<SalesOrderDto>>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetSalesOrdersHandler(
        ISalesOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<SalesOrderDto>> Handle(
        GetSalesOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);

        return _mapper.Map<List<SalesOrderDto>>(entities);
    }
}