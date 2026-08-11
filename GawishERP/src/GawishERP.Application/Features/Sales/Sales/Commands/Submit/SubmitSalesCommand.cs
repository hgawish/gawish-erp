using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Submit;

public sealed record SubmitSalesCommand(
    Guid SalesId)
    : IRequest<Result>;
