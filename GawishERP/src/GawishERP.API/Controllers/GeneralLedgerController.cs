using GawishERP.Application.Features.Accounting.GeneralLedger.Queries;
using GawishERP.Application.Features.Accounting.GeneralLedger.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GeneralLedgerController : ControllerBase
{
    private readonly IMediator _mediator;

    public GeneralLedgerController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetGeneralLedgerResponse>> Get(
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
                new GetGeneralLedgerQuery(
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