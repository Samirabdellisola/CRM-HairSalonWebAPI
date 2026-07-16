using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""UPDATE "users" SET "Name" = "Email" WHERE "Name" = '';""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "users");
        }
    }
}
