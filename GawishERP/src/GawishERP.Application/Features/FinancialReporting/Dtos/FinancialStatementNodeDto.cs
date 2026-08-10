using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Application.Features.FinancialReporting.Dtos;

public sealed class FinancialStatementNodeDto
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// الرصيد النهائى بعد التجميع أو المعادلات
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// مستوى العقدة داخل شجرة القوائم المالية
    /// </summary>
    public int Level { get; init; }

    /// <summary>
    /// معادلة الحساب
    /// مثال:
    /// 4000-5000
    /// أو
    /// Revenue-CostOfSales
    /// </summary>
    public string? Formula { get; init; }

    /// <summary>
    /// أبناء العقدة
    /// </summary>
    public List<FinancialStatementNodeDto> Children { get; } = new();
}