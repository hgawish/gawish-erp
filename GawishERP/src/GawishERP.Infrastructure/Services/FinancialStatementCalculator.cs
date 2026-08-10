using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.FinancialReporting.Dtos;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Services;

public sealed class FinancialStatementCalculator
    : IFinancialStatementCalculator
{
    private readonly ApplicationDbContext _context;

    private readonly IFinancialFormulaEvaluator _formulaEvaluator;

    public FinancialStatementCalculator(
        ApplicationDbContext context,
        IFinancialFormulaEvaluator formulaEvaluator)
    {
        _context = context;
        _formulaEvaluator = formulaEvaluator;
    }

    public async Task<List<FinancialStatementNodeDto>> CalculateAsync(
        FinancialStatementType statementType,
        DateTime asOfDate,
        CancellationToken cancellationToken = default)
    {
        //---------------------------------------------
        // Load Statement Nodes
        //---------------------------------------------

        var nodes = await _context.FinancialStatementNodes
            .AsNoTracking()
            .Where(x => x.StatementType == statementType)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        //---------------------------------------------
        // Load Account Balances
        //---------------------------------------------

        var balances = await _context.AccountBalances
            .Include(x => x.Account)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        //---------------------------------------------
        // Create DTO Lookup
        //---------------------------------------------

        var lookup = new Dictionary<Guid, FinancialStatementNodeDto>();

        foreach (var node in nodes)
        {
            var amount = balances
                .Where(x => x.Account.FinancialStatementNodeId == node.Id)
                .Sum(x => x.ClosingBalance);

            lookup[node.Id] = new FinancialStatementNodeDto
            {
                Id = node.Id,

                Code = node.Code,

                Name = node.Name,

                Level = node.Level,

                Formula = node.Formula,

                Amount = amount
            };
        }

        //---------------------------------------------
        // Build Tree
        //---------------------------------------------

        foreach (var node in nodes)
        {
            if (node.ParentId is null)
                continue;

            if (!lookup.TryGetValue(node.ParentId.Value, out var parent))
                continue;

            parent.Children.Add(
                lookup[node.Id]);
        }

        //---------------------------------------------
        // Code Lookup
        //---------------------------------------------

        var codeLookup =
            lookup.Values.ToDictionary(
                x => x.Code,
                x => x);

        //---------------------------------------------
        // Calculate Formulas
        //---------------------------------------------

        foreach (var node in lookup.Values)
        {
            if (string.IsNullOrWhiteSpace(node.Formula))
                continue;

            node.Amount =
                _formulaEvaluator.Evaluate(
                    node,
                    codeLookup);
        }

        //---------------------------------------------
        // Aggregate Children
        //---------------------------------------------

        foreach (var root in lookup.Values.Where(x => x.Level == 0))
        {
            Aggregate(root);
        }

        //---------------------------------------------
        // Return Root Nodes
        //---------------------------------------------

        return lookup.Values
            .Where(x => x.Level == 0)
            .OrderBy(x => x.Code)
            .ToList();
    }

    //-------------------------------------------------
    // Recursive Aggregation
    //-------------------------------------------------

    private static decimal Aggregate(
        FinancialStatementNodeDto node)
    {
        if (node.Children.Count == 0)
            return node.Amount;

        decimal total = node.Amount;

        foreach (var child in node.Children)
        {
            total += Aggregate(child);
        }

        node.Amount = total;

        return total;
    }
}