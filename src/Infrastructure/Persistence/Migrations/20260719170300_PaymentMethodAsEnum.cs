using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PaymentMethodAsEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                table: "payments",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE payments
                SET "PaymentMethodId" = CASE lower(trim("PaymentMethod"))
                    WHEN 'creditcard' THEN 0
                    WHEN 'credit_card' THEN 0
                    WHEN 'credit card' THEN 0
                    WHEN 'cash' THEN 1
                    WHEN 'banktransfer' THEN 2
                    WHEN 'bank_transfer' THEN 2
                    WHEN 'bank transfer' THEN 2
                    WHEN 'debitcard' THEN 3
                    WHEN 'debit_card' THEN 3
                    WHEN 'debit card' THEN 3
                    WHEN 'posterminal' THEN 4
                    WHEN 'pos_terminal' THEN 4
                    WHEN 'pos terminal' THEN 4
                    WHEN '0' THEN 0
                    WHEN '1' THEN 1
                    WHEN '2' THEN 2
                    WHEN '3' THEN 3
                    WHEN '4' THEN 4
                    ELSE 1
                END;
                """);

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodId",
                table: "payments",
                newName: "PaymentMethod");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethod",
                table: "payments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodName",
                table: "payments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE payments
                SET "PaymentMethodName" = CASE "PaymentMethod"
                    WHEN 0 THEN 'CreditCard'
                    WHEN 1 THEN 'Cash'
                    WHEN 2 THEN 'BankTransfer'
                    WHEN 3 THEN 'DebitCard'
                    WHEN 4 THEN 'PosTerminal'
                    ELSE 'Cash'
                END;
                """);

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodName",
                table: "payments",
                newName: "PaymentMethod");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "payments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);
        }
    }
}
