using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class UserTypechangedtoUserTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Users");

            migrationBuilder.AddColumn<int[]>(
                name: "UserTypes",
                table: "Users",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserTypes",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
