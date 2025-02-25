using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class ReasonRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Reasons_ReasonId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "Reasons");

            migrationBuilder.RenameColumn(
                name: "ReasonId",
                table: "Requests",
                newName: "CheckerId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_ReasonId",
                table: "Requests",
                newName: "IX_Requests_CheckerId");

            migrationBuilder.AddColumn<List<string>>(
                name: "Images",
                table: "Requests",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_CheckerId",
                table: "Requests",
                column: "CheckerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_CheckerId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "CheckerId",
                table: "Requests",
                newName: "ReasonId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_CheckerId",
                table: "Requests",
                newName: "IX_Requests_ReasonId");

            migrationBuilder.CreateTable(
                name: "Reasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Images = table.Column<List<string>>(type: "text[]", nullable: true),
                    ReasonType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reasons_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reasons_UserId",
                table: "Reasons",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Reasons_ReasonId",
                table: "Requests",
                column: "ReasonId",
                principalTable: "Reasons",
                principalColumn: "Id");
        }
    }
}
