using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Seed;

public static class FinancialStatementSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // =====================================================
        // Ensure the financial statement tree exists.
        // This method is intentionally idempotent: existing trees
        // are reused and account mappings are repaired on startup.
        // =====================================================

        var assets = await GetOrCreateNodeAsync(
            context,
            "BS-1000",
            "Assets",
            FinancialStatementType.BalanceSheet,
            NormalBalance.Debit,
            1,
            null,
            0);

        var currentAssets = await GetOrCreateNodeAsync(
            context,
            "BS-1100",
            "Current Assets",
            FinancialStatementType.BalanceSheet,
            NormalBalance.Debit,
            2,
            assets.Id,
            1);

        var liabilities = await GetOrCreateNodeAsync(
            context,
            "BS-2000",
            "Liabilities",
            FinancialStatementType.BalanceSheet,
            NormalBalance.Credit,
            3,
            null,
            0);

        var equity = await GetOrCreateNodeAsync(
            context,
            "BS-3000",
            "Equity",
            FinancialStatementType.BalanceSheet,
            NormalBalance.Credit,
            4,
            null,
            0);

        var revenue = await GetOrCreateNodeAsync(
            context,
            "IS-4000",
            "Revenue",
            FinancialStatementType.IncomeStatement,
            NormalBalance.Credit,
            5,
            null,
            0);

        var costOfSales = await GetOrCreateNodeAsync(
            context,
            "IS-5000",
            "Cost Of Sales",
            FinancialStatementType.IncomeStatement,
            NormalBalance.Debit,
            6,
            null,
            0);

        var operatingExpenses = await GetOrCreateNodeAsync(
            context,
            "IS-6000",
            "Operating Expenses",
            FinancialStatementType.IncomeStatement,
            NormalBalance.Debit,
            7,
            null,
            0);

        await context.SaveChangesAsync();

        // =====================================================
        // Repair / establish account -> financial statement node
        // mappings. This also fixes databases that were seeded before
        // FinancialStatementNodeId was introduced.
        // =====================================================

        var accounts = await context.Accounts.ToListAsync();

        foreach (var account in accounts)
        {
            Guid? nodeId = account.AccountType switch
            {
                AccountType.Asset => currentAssets.Id,
                AccountType.Liability => liabilities.Id,
                AccountType.Equity => equity.Id,
                AccountType.Revenue => revenue.Id,
                AccountType.Expense when account.Code == "5100" => costOfSales.Id,
                AccountType.Expense => operatingExpenses.Id,
                _ => null
            };

            if (nodeId.HasValue && account.FinancialStatementNodeId != nodeId)
                account.AssignFinancialStatementNode(nodeId.Value);
        }

        await context.SaveChangesAsync();
    }

    private static async Task<FinancialStatementNode> GetOrCreateNodeAsync(
        ApplicationDbContext context,
        string code,
        string name,
        FinancialStatementType statementType,
        NormalBalance normalBalance,
        int sortOrder,
        Guid? parentId,
        int level)
    {
        var node = await context.FinancialStatementNodes
            .FirstOrDefaultAsync(x => x.Code == code);

        if (node is null)
        {
            node = new FinancialStatementNode(
                code,
                name,
                statementType,
                normalBalance,
                sortOrder,
                true,
                false,
                parentId);

            node.SetHeader(true);
            node.SetLevel(level);

            context.FinancialStatementNodes.Add(node);
            return node;
        }

        // Existing database: repair the structural values that are needed
        // by reporting without replacing the existing node identity.
        node.SetHeader(true);
        node.SetLevel(level);

        return node;
    }
}
