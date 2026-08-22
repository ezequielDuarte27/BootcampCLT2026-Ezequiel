using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Full.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomersTransactionsAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "holder_name",
                table: "accounts");

            migrationBuilder.AddColumn<DateTime>(
                name: "closed_at",
                table: "accounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "accounts",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "customer_id",
                table: "accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    document_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    related_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_transactions_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_customer_id",
                table: "accounts",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_document_type_document_number",
                table: "customers",
                columns: new[] { "document_type", "document_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_account_id",
                table: "transactions",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_customer_id",
                table: "users",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_customers_customer_id",
                table: "accounts",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_accounts_customers_customer_id",
                table: "accounts");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropIndex(
                name: "ix_accounts_customer_id",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "closed_at",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "accounts");

            migrationBuilder.AddColumn<string>(
                name: "holder_name",
                table: "accounts",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }
    }
}
