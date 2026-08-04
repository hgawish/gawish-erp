using GawishERP.Application.Features.Accounting.AccountStatement.Queries;
using GawishERP.Application.Features.Accounting.AccountStatement.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AccountStatementController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountStatementController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetAccountStatementResponse>> Get(
        [FromQuery] Guid accountId,
        [FromQuery] Guid fiscalYearId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetAccountStatementQuery(
                    accountId,
                    fiscalYearId,
                    fromDate,
                    toDate,
                    companyId,
                    branchId),
                cancellationToken);

        return Ok(result);
    }
}