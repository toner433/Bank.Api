using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountBlockingAndPaymentOrderSignatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "signature_value",
                table: "payment_orders",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "signed_at",
                table: "payment_orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "signer_certificate_thumbprint",
                table: "payment_orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "admin_comment",
                table: "accounts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_blocked",
                table: "accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "signature_value",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "signed_at",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "signer_certificate_thumbprint",
                table: "payment_orders");

            migrationBuilder.DropColumn(
                name: "admin_comment",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "is_blocked",
                table: "accounts");
        }
    }
}
