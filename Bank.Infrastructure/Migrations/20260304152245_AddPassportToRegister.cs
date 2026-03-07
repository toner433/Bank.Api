using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPassportToRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "cards");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "sessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_blocked",
                table: "cards",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "sessions");

            migrationBuilder.DropColumn(
                name: "is_blocked",
                table: "cards");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "cards",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "active");
        }
    }
}
