using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIsFrozen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFrozen",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFrozen",
                table: "users");
        }
    }
}
