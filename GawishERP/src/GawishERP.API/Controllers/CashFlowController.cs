using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.FinancialReporting.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CashFlowController : ControllerBase
{
    private readonly IFinancialReportingService _financialReportingService;

    public CashFlowController(IFinancialReportingService financialReportingService)
    {
        _financialReportingService = financialReportingService;
    }

    [HttpGet]
    public async Task<ActionResult<CashFlowDto>> Get(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await _financialReportingService.GetCashFlowAsync(
            from,
            to,
            cancellationToken);

        return Ok(result);
    }
}
