using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class ReasonAndRequestEntitiesChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Reasons");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Requests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Reasons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Reasons");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Reasons",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
