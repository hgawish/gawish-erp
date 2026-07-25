using MediatR;
using GawishERP.Application.Common.Results;

namespace GawishERP.Application.Common.CQRS;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}