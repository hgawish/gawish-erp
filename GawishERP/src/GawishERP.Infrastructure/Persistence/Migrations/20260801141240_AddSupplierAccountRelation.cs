using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GawishERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierAccountRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Suppliers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_AccountId",
                table: "Suppliers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountId",
                table: "Customers",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Accounts_AccountId",
                table: "Customers",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Accounts_AccountId",
                table: "Suppliers",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Accounts_AccountId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Accounts_AccountId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_AccountId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AccountId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Customers");
        }
    }
}
