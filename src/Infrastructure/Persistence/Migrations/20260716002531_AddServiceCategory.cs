using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceCategoryId",
                table: "services",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "service_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_service_categories_branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_services_ServiceCategoryId",
                table: "services",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_service_categories_BranchId_Name",
                table: "service_categories",
                columns: new[] { "BranchId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_services_service_categories_ServiceCategoryId",
                table: "services",
                column: "ServiceCategoryId",
                principalTable: "service_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_service_categories_ServiceCategoryId",
                table: "services");

            migrationBuilder.DropTable(
                name: "service_categories");

            migrationBuilder.DropIndex(
                name: "IX_services_ServiceCategoryId",
                table: "services");

            migrationBuilder.DropColumn(
                name: "ServiceCategoryId",
                table: "services");
        }
    }
}
