using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixRolesAddBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fixes any user already persisted with the old "Admin" role string
            // (from the previous bootstrap endpoint) before the enum is renamed in code.
            migrationBuilder.Sql("UPDATE users SET \"Role\" = 'CentralOffice' WHERE \"Role\" = 'Admin';");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsFrozen = table.Column<bool>(type: "boolean", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_branches_users_AdminId",
                        column: x => x.AdminId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_BranchId",
                table: "users",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_branches_AdminId",
                table: "branches",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_branches_Name",
                table: "branches",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_branches_BranchId",
                table: "users",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_branches_BranchId",
                table: "users");

            migrationBuilder.DropTable(
                name: "branches");

            migrationBuilder.DropIndex(
                name: "IX_users_BranchId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "users");
        }
    }
}
