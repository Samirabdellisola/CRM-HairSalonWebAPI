using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FlattenOrderSingleService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ServicePrice",
                table: "orders",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE orders o
                SET "ServiceId" = i."ServiceId",
                    "ServiceName" = i."ServiceName",
                    "ServicePrice" = i."ServicePrice",
                    "TotalPrice" = i."ServicePrice"
                FROM (
                    SELECT DISTINCT ON ("OrderId")
                        "OrderId", "ServiceId", "ServiceName", "ServicePrice"
                    FROM order_items
                    ORDER BY "OrderId", "CreatedAt"
                ) i
                WHERE o."Id" = i."OrderId";
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM payments
                WHERE "OrderId" IN (SELECT "Id" FROM orders WHERE "ServiceId" IS NULL);
                """);

            migrationBuilder.Sql(
                """
                UPDATE orders SET "PaymentId" = NULL WHERE "ServiceId" IS NULL;
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM orders WHERE "ServiceId" IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceId",
                table: "orders",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ServicePrice",
                table: "orders",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.CreateIndex(
                name: "IX_orders_ServiceId",
                table: "orders",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_services_ServiceId",
                table: "orders",
                column: "ServiceId",
                principalTable: "services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_services_ServiceId",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_ServiceId",
                table: "orders");

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ServicePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_items_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_items_services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO order_items ("Id", "OrderId", "ServiceId", "ServiceName", "ServicePrice", "CreatedAt")
                SELECT gen_random_uuid(), o."Id", o."ServiceId", o."ServiceName", o."ServicePrice", o."CreatedAt"
                FROM orders o;
                """);

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ServicePrice",
                table: "orders");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_OrderId_ServiceId",
                table: "order_items",
                columns: new[] { "OrderId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_items_ServiceId",
                table: "order_items",
                column: "ServiceId");
        }
    }
}
