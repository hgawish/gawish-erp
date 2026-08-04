using GawishERP.Application.Features.Accounting.TrialBalance.Queries;
using GawishERP.Application.Features.Accounting.TrialBalance.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TrialBalanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public TrialBalanceController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetTrialBalanceResponse>> Get(
        [FromQuery] Guid fiscalYearId,
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetTrialBalanceQuery(
                    fiscalYearId,
                    companyId,
                    branchId),
                cancellationToken);

        return Ok(result);
    }
}