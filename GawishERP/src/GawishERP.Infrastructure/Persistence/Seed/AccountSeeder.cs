
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Seed;

public static class AccountSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        // =====================================================
        // لا نعيد إنشاء الحسابات إذا كانت موجودة
        // =====================================================

        if (await context.Accounts.AnyAsync())
            return;

        // =====================================================
        // LEVEL 1
        // الحسابات الرئيسية
        // =====================================================

        var assets = new Account(
            "1000",
            "الأصول",
            AccountType.Asset,
            AccountNature.Debit,
            false);

        var liabilities = new Account(
            "2000",
            "الخصوم",
            AccountType.Liability,
            AccountNature.Credit,
            false);

        var equity = new Account(
            "3000",
            "حقوق الملكية",
            AccountType.Equity,
            AccountNature.Credit,
            false);

        var revenue = new Account(
            "4000",
            "الإيرادات",
            AccountType.Revenue,
            AccountNature.Credit,
            false);

        var expenses = new Account(
            "5000",
            "المصروفات",
            AccountType.Expense,
            AccountNature.Debit,
            false);

        context.Accounts.AddRange(
            assets,
            liabilities,
            equity,
            revenue,
            expenses);

        // مهم جدًا:
        // حفظ الـ Parent Accounts أولًا
        await context.SaveChangesAsync();

        // =====================================================
        // LEVEL 2
        // الحسابات الفرعية الرئيسية
        // =====================================================

        // -------------------------
        // Assets
        // -------------------------

        var currentAssets = new Account(
            "1100",
            "الأصول المتداولة",
            AccountType.Asset,
            AccountNature.Debit,
            false,
            assets.Id);

        var fixedAssets = new Account(
            "1200",
            "الأصول غير المتداولة",
            AccountType.Asset,
            AccountNature.Debit,
            false,
            assets.Id);

        // -------------------------
        // Liabilities
        // -------------------------

        var currentLiabilities = new Account(
            "2100",
            "الخصوم المتداولة",
            AccountType.Liability,
            AccountNature.Credit,
            false,
            liabilities.Id);

        var longTermLiabilities = new Account(
            "2200",
            "الخصوم طويلة الأجل",
            AccountType.Liability,
            AccountNature.Credit,
            false,
            liabilities.Id);

        // -------------------------
        // Expenses
        // -------------------------

        var administrativeExpenses = new Account(
            "5200",
            "المصروفات الإدارية",
            AccountType.Expense,
            AccountNature.Debit,
            false,
            expenses.Id);

        var sellingExpenses = new Account(
            "5300",
            "مصروفات البيع والتسويق",
            AccountType.Expense,
            AccountNature.Debit,
            false,
            expenses.Id);

        context.Accounts.AddRange(
            currentAssets,
            fixedAssets,
            currentLiabilities,
            longTermLiabilities,
            administrativeExpenses,
            sellingExpenses);

        // حفظ المستوى الثاني قبل إنشاء أبنائه
        await context.SaveChangesAsync();

        // =====================================================
        // LEVEL 3
        // الحسابات النهائية / Posting Accounts
        // =====================================================

        // =====================================================
        // Current Assets
        // =====================================================

        var cash = new Account(
            "1110",
            "الصندوق",
            AccountType.Asset,
            AccountNature.Debit,
            true,
            currentAssets.Id,
            null,
            true);

        var bank = new Account(
            "1120",
            "البنوك",
            AccountType.Asset,
            AccountNature.Debit,
            true,
            currentAssets.Id);

        var customers = new Account(
            "1130",
            "العملاء",
            AccountType.Asset,
            AccountNature.Debit,
            true,
            currentAssets.Id);

        var inventory = new Account(
            "1140",
            "المخزون",
            AccountType.Asset,
            AccountNature.Debit,
            true,
            currentAssets.Id);

        var inventoryInTransit = new Account(
            "1150",
            "بضاعة بالطريق",
            AccountType.Asset,
            AccountNature.Debit,
            true,
            currentAssets.Id);

        var advancePayments = new Account(
            "1160",
            "دفعات مقدمة للموردين",
            AccountType.Asset,
            AccountNature.Debit,
            true,
            currentAssets.Id);

        // =====================================================
        // Fixed Assets
        // =====================================================

        var vehicles = new Account(
            "1210",
            "السيارات",
            AccountType.Asset,
            AccountNature.Debit,
            true,
            fixedAssets.Id);

        var equipment = new Account(
            "1220",
            "المعدات",
            AccountType.Asset,
            AccountNature.Debit,
            true,
            fixedAssets.Id);

        var furniture = new Account(
            "1230",
            "الأثاث والتجهيزات",
            AccountType.Asset,
            AccountNature.Debit,
            true,
            fixedAssets.Id);

        // =====================================================
        // Current Liabilities
        // =====================================================

        var suppliers = new Account(
            "2110",
            "الموردون",
            AccountType.Liability,
            AccountNature.Credit,
            true,
            currentLiabilities.Id);

        var accruedExpenses = new Account(
            "2120",
            "مصروفات مستحقة",
            AccountType.Liability,
            AccountNature.Credit,
            true,
            currentLiabilities.Id);

        var customerAdvances = new Account(
            "2130",
            "دفعات مقدمة من العملاء",
            AccountType.Liability,
            AccountNature.Credit,
            true,
            currentLiabilities.Id);

        var vatPayable = new Account(
            "2140",
            "ضريبة القيمة المضافة المستحقة",
            AccountType.Liability,
            AccountNature.Credit,
            true,
            currentLiabilities.Id);

        // =====================================================
        // Long Term Liabilities
        // =====================================================

        var loans = new Account(
            "2210",
            "القروض",
            AccountType.Liability,
            AccountNature.Credit,
            true,
            longTermLiabilities.Id);

        // =====================================================
        // Equity
        // =====================================================

        var capital = new Account(
            "3100",
            "رأس المال",
            AccountType.Equity,
            AccountNature.Credit,
            true,
            equity.Id);

        var retainedEarnings = new Account(
            "3200",
            "الأرباح المحتجزة",
            AccountType.Equity,
            AccountNature.Credit,
            true,
            equity.Id);

        var currentYearProfit = new Account(
            "3300",
            "أرباح وخسائر العام الحالي",
            AccountType.Equity,
            AccountNature.Credit,
            true,
            equity.Id);

        // =====================================================
        // Revenue
        // =====================================================

        var sales = new Account(
            "4100",
            "المبيعات",
            AccountType.Revenue,
            AccountNature.Credit,
            true,
            revenue.Id);

        var salesReturns = new Account(
            "4200",
            "مردودات المبيعات",
            AccountType.Revenue,
            AccountNature.Debit,
            true,
            revenue.Id);

        var otherRevenue = new Account(
            "4300",
            "إيرادات أخرى",
            AccountType.Revenue,
            AccountNature.Credit,
            true,
            revenue.Id);

        // =====================================================
        // Expenses
        // =====================================================

        var costOfGoodsSold = new Account(
            "5100",
            "تكلفة البضاعة المباعة",
            AccountType.Expense,
            AccountNature.Debit,
            true,
            expenses.Id);

        var salaries = new Account(
            "5210",
            "الرواتب والأجور",
            AccountType.Expense,
            AccountNature.Debit,
            true,
            administrativeExpenses.Id);

        var rent = new Account(
            "5220",
            "الإيجارات",
            AccountType.Expense,
            AccountNature.Debit,
            true,
            administrativeExpenses.Id);

        var utilities = new Account(
            "5230",
            "المرافق",
            AccountType.Expense,
            AccountNature.Debit,
            true,
            administrativeExpenses.Id);

        var marketing = new Account(
            "5310",
            "التسويق والإعلان",
            AccountType.Expense,
            AccountNature.Debit,
            true,
            sellingExpenses.Id);

        var transportation = new Account(
            "5320",
            "النقل والمواصلات",
            AccountType.Expense,
            AccountNature.Debit,
            true,
            sellingExpenses.Id);

        var bankCharges = new Account(
            "5400",
            "مصروفات بنكية",
            AccountType.Expense,
            AccountNature.Debit,
            true,
            expenses.Id);

        // =====================================================
        // Add LEVEL 3
        // =====================================================

        context.Accounts.AddRange(
            // Current Assets
            cash,
            bank,
            customers,
            inventory,
            inventoryInTransit,
            advancePayments,

            // Fixed Assets
            vehicles,
            equipment,
            furniture,

            // Current Liabilities
            suppliers,
            accruedExpenses,
            customerAdvances,
            vatPayable,

            // Long Term Liabilities
            loans,

            // Equity
            capital,
            retainedEarnings,
            currentYearProfit,

            // Revenue
            sales,
            salesReturns,
            otherRevenue,

            // Expenses
            costOfGoodsSold,
            salaries,
            rent,
            utilities,
            marketing,
            transportation,
            bankCharges);

        // =====================================================
        // Save LEVEL 3
        // =====================================================

        await context.SaveChangesAsync();
    }
}
