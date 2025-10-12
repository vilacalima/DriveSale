using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Infrastructure.Persistence;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20251012000100_AddClientToSale")]
    public partial class AddClientToSale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Add column as nullable to allow backfill
            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "sales",
                type: "uuid",
                nullable: true);

            // 2) Backfill by matching BuyerCpf with clients.Cpf
            migrationBuilder.Sql("UPDATE \"sales\" s SET \"ClientId\" = c.\"Id\" FROM \"clients\" c WHERE c.\"Cpf\" = s.\"BuyerCpf\";");

            // 3) Set NOT NULL after backfill
            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "sales",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            // 4) Create index and FK
            migrationBuilder.CreateIndex(
                name: "IX_sales_ClientId",
                table: "sales",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_sales_clients_ClientId",
                table: "sales",
                column: "ClientId",
                principalTable: "clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sales_clients_ClientId",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_sales_ClientId",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "sales");
        }
    }
}

