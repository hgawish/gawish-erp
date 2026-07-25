using MediatR;
using GawishERP.Application.Common.Results;

namespace GawishERP.Application.Common.CQRS;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}