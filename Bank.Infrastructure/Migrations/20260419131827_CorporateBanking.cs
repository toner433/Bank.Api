using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorporateBanking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "accounts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "organization_id",
                table: "accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    inn = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    kpp = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    legal_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organization_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_organization_members_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_organization_members_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    recipient_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    recipient_inn = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    recipient_account_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    purpose = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    executed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_orders_accounts_FromAccountId",
                        column: x => x.FromAccountId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_payment_orders_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payment_orders_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "time_deposits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    DepositAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    principal = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    annual_rate_percent = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    term_months = table.Column<int>(type: "integer", nullable: false),
                    opened_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    maturity_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_deposits", x => x.id);
                    table.ForeignKey(
                        name: "FK_time_deposits_accounts_DepositAccountId",
                        column: x => x.DepositAccountId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_time_deposits_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_time_deposits_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_organization_id",
                table: "accounts",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_organization_members_OrganizationId_UserId",
                table: "organization_members",
                columns: new[] { "OrganizationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_organization_members_UserId",
                table: "organization_members",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_organizations_inn",
                table: "organizations",
                column: "inn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_orders_CreatedByUserId",
                table: "payment_orders",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_orders_FromAccountId",
                table: "payment_orders",
                column: "FromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_orders_OrganizationId",
                table: "payment_orders",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_time_deposits_DepositAccountId",
                table: "time_deposits",
                column: "DepositAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_time_deposits_OrganizationId",
                table: "time_deposits",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_time_deposits_UserId",
                table: "time_deposits",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_organizations_organization_id",
                table: "accounts",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_organizations_organization_id",
                table: "accounts");

            migrationBuilder.DropTable(
                name: "organization_members");

            migrationBuilder.DropTable(
                name: "payment_orders");

            migrationBuilder.DropTable(
                name: "time_deposits");

            migrationBuilder.DropTable(
                name: "organizations");

            migrationBuilder.DropIndex(
                name: "IX_accounts_organization_id",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "organization_id",
                table: "accounts");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
