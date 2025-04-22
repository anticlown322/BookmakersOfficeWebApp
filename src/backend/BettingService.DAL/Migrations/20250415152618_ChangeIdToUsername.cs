using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BettingService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdToUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bets_UserId",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bets");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Bets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Bets_Username",
                table: "Bets",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bets_Username",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Bets");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Bets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bets_UserId",
                table: "Bets",
                column: "UserId");
        }
    }
}
