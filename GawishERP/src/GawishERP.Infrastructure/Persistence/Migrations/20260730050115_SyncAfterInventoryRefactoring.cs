using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GawishERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncAfterInventoryRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_ProductId_WarehouseId_TransactionDate",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_WarehouseId",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "IsPosted",
                table: "PurchaseHeaders");

            migrationBuilder.DropColumn(
                name: "IsPosted",
                table: "OpeningBalanceHeaders");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "StockTransactions",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "StockTransactions",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "PurchaseLines",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PurchaseHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExchangeRate",
                table: "PurchaseHeaders",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ProductId",
                table: "StockTransactions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_TransactionType",
                table: "StockTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_WarehouseId_ProductId_TransactionDate",
                table: "StockTransactions",
                columns: new[] { "WarehouseId", "ProductId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseHeaders_DocumentDate",
                table: "PurchaseHeaders",
                column: "DocumentDate");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseHeaders_InvoiceDate",
                table: "PurchaseHeaders",
                column: "InvoiceDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_ProductId",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_TransactionType",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_WarehouseId_ProductId_TransactionDate",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseHeaders_DocumentDate",
                table: "PurchaseHeaders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseHeaders_InvoiceDate",
                table: "PurchaseHeaders");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "StockTransactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "StockTransactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "PurchaseLines",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PurchaseHeaders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExchangeRate",
                table: "PurchaseHeaders",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)",
                oldPrecision: 18,
                oldScale: 8);

            migrationBuilder.AddColumn<bool>(
                name: "IsPosted",
                table: "PurchaseHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPosted",
                table: "OpeningBalanceHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ProductId_WarehouseId_TransactionDate",
                table: "StockTransactions",
                columns: new[] { "ProductId", "WarehouseId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_WarehouseId",
                table: "StockTransactions",
                column: "WarehouseId");
        }
    }
}
