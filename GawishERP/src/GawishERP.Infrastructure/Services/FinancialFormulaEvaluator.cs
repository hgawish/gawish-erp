using System.Data;
using System.Globalization;
using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.FinancialReporting.Dtos;

namespace GawishERP.Infrastructure.Services;

public sealed class FinancialFormulaEvaluator
    : IFinancialFormulaEvaluator
{
    public decimal Evaluate(
        FinancialStatementNodeDto node,
        IReadOnlyDictionary<string, FinancialStatementNodeDto> lookup)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        if (lookup is null)
            throw new ArgumentNullException(nameof(lookup));

        // إذا لم توجد معادلة نرجع الرصيد مباشرة
        if (string.IsNullOrWhiteSpace(node.Formula))
            return node.Amount;

        var expression = node.Formula;

        // استبدال أكواد العقد بالقيم الفعلية
        foreach (var item in lookup)
        {
            expression = expression.Replace(
                item.Key,
                item.Value.Amount.ToString(CultureInfo.InvariantCulture));
        }

        try
        {
            var table = new DataTable();

            var result = table.Compute(expression, "");

            return Convert.ToDecimal(
                result,
                CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Invalid financial formula: {node.Formula}",
                ex);
        }
    }
}