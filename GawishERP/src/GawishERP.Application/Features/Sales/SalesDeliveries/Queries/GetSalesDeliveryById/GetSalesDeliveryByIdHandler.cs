using AutoMapper;
using GawishERP.Application.Features.Sales.SalesDeliveries.Dtos;
using GawishERP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Queries.GetSalesDeliveryById;

public sealed class GetSalesDeliveryByIdHandler
    : IRequestHandler<GetSalesDeliveryByIdQuery, SalesDeliveryDto>
{
    private readonly ISalesDeliveryRepository _repository;
    private readonly IMapper _mapper;

    public GetSalesDeliveryByIdHandler(
        ISalesDeliveryRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SalesDeliveryDto> Handle(
        GetSalesDeliveryByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException(
                "Sales Delivery ID cannot be empty.",
                nameof(request.Id));
        }

        var entity =
            await _repository.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (entity is null)
        {
            throw new KeyNotFoundException(
                "Sales Delivery was not found.");
        }

        return _mapper.Map<SalesDeliveryDto>(entity);
    }
}