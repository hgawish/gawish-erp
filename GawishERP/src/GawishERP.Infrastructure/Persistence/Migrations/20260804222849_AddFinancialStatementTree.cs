using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GawishERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialStatementTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FinancialStatementNodeId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FinancialStatementNodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StatementType = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NormalBalance = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false),
                    Formula = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsHeader = table.Column<bool>(type: "bit", nullable: false),
                    IsTotal = table.Column<bool>(type: "bit", nullable: false),
                    AllowPosting = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialStatementNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialStatementNodes_FinancialStatementNodes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "FinancialStatementNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_FinancialStatementNodeId",
                table: "Accounts",
                column: "FinancialStatementNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialStatementNodes_Code",
                table: "FinancialStatementNodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialStatementNodes_ParentId",
                table: "FinancialStatementNodes",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_FinancialStatementNodes_FinancialStatementNodeId",
                table: "Accounts",
                column: "FinancialStatementNodeId",
                principalTable: "FinancialStatementNodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_FinancialStatementNodes_FinancialStatementNodeId",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "FinancialStatementNodes");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_FinancialStatementNodeId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "FinancialStatementNodeId",
                table: "Accounts");
        }
    }
}
