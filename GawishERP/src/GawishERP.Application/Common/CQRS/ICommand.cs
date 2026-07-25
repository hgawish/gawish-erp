using MediatR;
using GawishERP.Application.Common.Results;

namespace GawishERP.Application.Common.CQRS;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}