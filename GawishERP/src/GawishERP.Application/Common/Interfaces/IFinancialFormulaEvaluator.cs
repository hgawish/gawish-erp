using GawishERP.Application.Features.FinancialReporting.Dtos;

namespace GawishERP.Application.Common.Interfaces;

public interface IFinancialFormulaEvaluator
{
    decimal Evaluate(
        FinancialStatementNodeDto node,
        IReadOnlyDictionary<string, FinancialStatementNodeDto> lookup);
}