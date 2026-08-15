using GawishERP.Application.Features.Accounting.BalanceSheet.Queries;
using GawishERP.Application.Features.Accounting.BalanceSheet.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BalanceSheetController : ControllerBase
{
    private readonly IMediator _mediator;

    public BalanceSheetController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetBalanceSheetResponse>> Get(
        [FromQuery] Guid fiscalYearId,
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetBalanceSheetQuery(
                fiscalYearId,
                companyId,
                branchId),
            cancellationToken);

        return Ok(result);
    }
}
