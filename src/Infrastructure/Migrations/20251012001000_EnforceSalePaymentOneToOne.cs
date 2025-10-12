using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Infrastructure.Persistence;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20251012001000_EnforceSalePaymentOneToOne")]
    public partial class EnforceSalePaymentOneToOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adjust BuyerCpf length to 11 (only digits)
            migrationBuilder.AlterColumn<string>(
                name: "BuyerCpf",
                table: "sales",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(14)",
                oldMaxLength: 14);

            // Ensure unique PaymentId (1:1 Sale-Payment)
            migrationBuilder.DropIndex(
                name: "IX_sales_PaymentId",
                table: "sales");

            migrationBuilder.CreateIndex(
                name: "IX_sales_PaymentId",
                table: "sales",
                column: "PaymentId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sales_PaymentId",
                table: "sales");

            migrationBuilder.CreateIndex(
                name: "IX_sales_PaymentId",
                table: "sales",
                column: "PaymentId");

            migrationBuilder.AlterColumn<string>(
                name: "BuyerCpf",
                table: "sales",
                type: "character varying(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11);
        }
    }
}

