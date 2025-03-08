using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class FixUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserTypes",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(int[]),
                oldType: "integer[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int[]>(
                name: "UserTypes",
                table: "Users",
                type: "integer[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
