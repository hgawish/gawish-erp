using GawishERP.Application.Features.Accounting.Reports.Profit_and_Loss;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProfitAndLossController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfitAndLossController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetProfitAndLossResponse>> Get(
        [FromQuery] Guid fiscalYearId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetProfitAndLossQuery(
                fiscalYearId,
                fromDate,
                toDate,
                companyId,
                branchId),
            cancellationToken);

        return Ok(result);
    }
}
