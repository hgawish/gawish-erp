using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GawishERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseReturn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseReturnHeaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReturnHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseReturnHeaders_PurchaseHeaders_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "PurchaseHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReturnHeaders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReturnHeaders_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseReturnLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseReturnHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReturnLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseReturnLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReturnLines_PurchaseLines_PurchaseLineId",
                        column: x => x.PurchaseLineId,
                        principalTable: "PurchaseLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReturnLines_PurchaseReturnHeaders_PurchaseReturnHeaderId",
                        column: x => x.PurchaseReturnHeaderId,
                        principalTable: "PurchaseReturnHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnHeaders_PurchaseId",
                table: "PurchaseReturnHeaders",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnHeaders_SupplierId",
                table: "PurchaseReturnHeaders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnHeaders_WarehouseId",
                table: "PurchaseReturnHeaders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnLines_ProductId",
                table: "PurchaseReturnLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnLines_PurchaseLineId",
                table: "PurchaseReturnLines",
                column: "PurchaseLineId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnLines_PurchaseReturnHeaderId",
                table: "PurchaseReturnLines",
                column: "PurchaseReturnHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseReturnLines");

            migrationBuilder.DropTable(
                name: "PurchaseReturnHeaders");
        }
    }
}
