using AutoMapper;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Application.Features.Sales.SalesOrders.Dtos;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Queries.GetSalesOrderById;

public sealed class GetSalesOrderByIdHandler
    : IRequestHandler<GetSalesOrderByIdQuery, SalesOrderDto>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetSalesOrderByIdHandler(
        ISalesOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SalesOrderDto> Handle(
        GetSalesOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (entity is null)
            throw new Exception("Sales Order not found.");

        return _mapper.Map<SalesOrderDto>(entity);
    }
}