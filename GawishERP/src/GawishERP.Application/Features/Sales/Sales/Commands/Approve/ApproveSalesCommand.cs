using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Approve;

public sealed record ApproveSalesCommand(
    Guid SalesId)
    : IRequest<Result>;
