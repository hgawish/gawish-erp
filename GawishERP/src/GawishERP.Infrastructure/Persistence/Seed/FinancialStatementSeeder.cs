using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Seed;

public static class FinancialStatementSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.FinancialStatementNodes.AnyAsync())
            return;

        // ===============================
        // BALANCE SHEET
        // ===============================

        var assets = new FinancialStatementNode(
            "BS-1000",
            "Assets",
            FinancialStatementType.BalanceSheet,
            NormalBalance.Debit,
            1,
            true,
            false);

        assets.SetHeader(true);
        assets.SetLevel(0);

        var currentAssets = new FinancialStatementNode(
            "BS-1100",
            "Current Assets",
            FinancialStatementType.BalanceSheet,
            NormalBalance.Debit,
            2,
            true,
            false,
            assets.Id);

        currentAssets.SetHeader(true);
        currentAssets.SetLevel(1);

        var liabilities = new FinancialStatementNode(
            "BS-2000",
            "Liabilities",
            FinancialStatementType.BalanceSheet,
            NormalBalance.Credit,
            3,
            true,
            false);

        liabilities.SetHeader(true);
        liabilities.SetLevel(0);

        var equity = new FinancialStatementNode(
            "BS-3000",
            "Equity",
            FinancialStatementType.BalanceSheet,
            NormalBalance.Credit,
            4,
            true,
            false);

        equity.SetHeader(true);
        equity.SetLevel(0);

        // ===============================
        // INCOME STATEMENT
        // ===============================

        var revenue = new FinancialStatementNode(
            "IS-4000",
            "Revenue",
            FinancialStatementType.IncomeStatement,
            NormalBalance.Credit,
            5,
            true,
            false);

        revenue.SetHeader(true);
        revenue.SetLevel(0);

        var costOfSales = new FinancialStatementNode(
            "IS-5000",
            "Cost Of Sales",
            FinancialStatementType.IncomeStatement,
            NormalBalance.Debit,
            6,
            true,
            false);

        costOfSales.SetHeader(true);
        costOfSales.SetLevel(0);

        var operatingExpenses = new FinancialStatementNode(
            "IS-6000",
            "Operating Expenses",
            FinancialStatementType.IncomeStatement,
            NormalBalance.Debit,
            7,
            true,
            false);

        operatingExpenses.SetHeader(true);
        operatingExpenses.SetLevel(0);

        context.FinancialStatementNodes.AddRange(
            assets,
            currentAssets,
            liabilities,
            equity,
            revenue,
            costOfSales,
            operatingExpenses);

        await context.SaveChangesAsync();
    }
}