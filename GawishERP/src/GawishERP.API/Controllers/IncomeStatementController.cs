using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.FinancialReporting.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class IncomeStatementController : ControllerBase
{
    private readonly IFinancialReportingService _financialReportingService;

    public IncomeStatementController(
        IFinancialReportingService financialReportingService)
    {
        _financialReportingService = financialReportingService;
    }

    [HttpGet]
    public async Task<ActionResult<IncomeStatementDto>> Get(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await _financialReportingService.GetIncomeStatementAsync(
            from,
            to,
            cancellationToken);

        return Ok(result);
    }
}
