using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GawishERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class JournalReverseRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FiscalYear",
                table: "NumberSeries");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Guid>(
                name: "FiscalYearId",
                table: "NumberSeries",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "JournalEntryHeaders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "JournalEntryHeaders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentType",
                table: "JournalEntryHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "FiscalYearId",
                table: "JournalEntryHeaders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsReversed",
                table: "JournalEntryHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalJournalEntryId",
                table: "JournalEntryHeaders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReversedByJournalEntryId",
                table: "JournalEntryHeaders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FiscalYears",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsOpen = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FiscalYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OpeningDebit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    OpeningCredit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CurrentDebit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CurrentCredit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ClosingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountBalances_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountBalances_FiscalYears_FiscalYearId",
                        column: x => x.FiscalYearId,
                        principalTable: "FiscalYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LedgerTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JournalEntryHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JournalEntryLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FiscalYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RunningBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LedgerTransactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LedgerTransactions_FiscalYears_FiscalYearId",
                        column: x => x.FiscalYearId,
                        principalTable: "FiscalYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LedgerTransactions_JournalEntryHeaders_JournalEntryHeaderId",
                        column: x => x.JournalEntryHeaderId,
                        principalTable: "JournalEntryHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LedgerTransactions_JournalEntryLines_JournalEntryLineId",
                        column: x => x.JournalEntryLineId,
                        principalTable: "JournalEntryLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryHeaders_OriginalJournalEntryId",
                table: "JournalEntryHeaders",
                column: "OriginalJournalEntryId",
                unique: true,
                filter: "[OriginalJournalEntryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBalances_AccountId_FiscalYearId_CompanyId_BranchId",
                table: "AccountBalances",
                columns: new[] { "AccountId", "FiscalYearId", "CompanyId", "BranchId" },
                unique: true,
                filter: "[CompanyId] IS NOT NULL AND [BranchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBalances_FiscalYearId",
                table: "AccountBalances",
                column: "FiscalYearId");

            migrationBuilder.CreateIndex(
                name: "IX_FiscalYears_Code",
                table: "FiscalYears",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_AccountId_PostingDate",
                table: "LedgerTransactions",
                columns: new[] { "AccountId", "PostingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_DocumentNumber",
                table: "LedgerTransactions",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_FiscalYearId",
                table: "LedgerTransactions",
                column: "FiscalYearId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_JournalEntryHeaderId",
                table: "LedgerTransactions",
                column: "JournalEntryHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_JournalEntryLineId",
                table: "LedgerTransactions",
                column: "JournalEntryLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryHeaders_JournalEntryHeaders_OriginalJournalEntryId",
                table: "JournalEntryHeaders",
                column: "OriginalJournalEntryId",
                principalTable: "JournalEntryHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryHeaders_JournalEntryHeaders_OriginalJournalEntryId",
                table: "JournalEntryHeaders");

            migrationBuilder.DropTable(
                name: "AccountBalances");

            migrationBuilder.DropTable(
                name: "LedgerTransactions");

            migrationBuilder.DropTable(
                name: "FiscalYears");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryHeaders_OriginalJournalEntryId",
                table: "JournalEntryHeaders");

            migrationBuilder.DropColumn(
                name: "FiscalYearId",
                table: "NumberSeries");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "JournalEntryHeaders");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "JournalEntryHeaders");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "JournalEntryHeaders");

            migrationBuilder.DropColumn(
                name: "FiscalYearId",
                table: "JournalEntryHeaders");

            migrationBuilder.DropColumn(
                name: "IsReversed",
                table: "JournalEntryHeaders");

            migrationBuilder.DropColumn(
                name: "OriginalJournalEntryId",
                table: "JournalEntryHeaders");

            migrationBuilder.DropColumn(
                name: "ReversedByJournalEntryId",
                table: "JournalEntryHeaders");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "FiscalYear",
                table: "NumberSeries",
                type: "int",
                nullable: true);
        }
    }
}
