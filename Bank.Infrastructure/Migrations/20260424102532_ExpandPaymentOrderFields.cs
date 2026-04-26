using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandPaymentOrderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "document_date",
                table: "payment_orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "document_number",
                table: "payment_orders",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "payment_priority",
                table: "payment_orders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_type",
                table: "payment_orders",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipient_bank_bik",
                table: "payment_orders",
                type: "character varying(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipient_bank_name",
                table: "payment_orders",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipient_kpp",
                table: "payment_orders",
                type: "character varying(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "vat_amount",
                table: "payment_orders",
                type: "numeric(15,2)",
                precision: 15,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vat_type",
                table: "payment_orders",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "document_date",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "document_number",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "payment_priority",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "payment_type",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "recipient_bank_bik",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "recipient_bank_name",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "recipient_kpp",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "vat_amount",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "vat_type",
                table: "payment_orders");
        }
    }
}
