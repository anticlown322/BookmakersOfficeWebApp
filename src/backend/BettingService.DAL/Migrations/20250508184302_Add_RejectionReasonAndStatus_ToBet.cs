using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BettingService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Add_RejectionReasonAndStatus_ToBet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Bets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Bets");
        }
    }
}
