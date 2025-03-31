using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraintsToBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceTransaction_AspNetUsers_UserId",
                table: "BalanceTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_BalanceTransaction_UserBalance_UserBalanceId",
                table: "BalanceTransaction");

            migrationBuilder.DropIndex(
                name: "IX_BalanceTransaction_UserBalanceId",
                table: "BalanceTransaction");

            migrationBuilder.DropColumn(
                name: "UserBalanceId",
                table: "BalanceTransaction");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentAmount",
                table: "UserBalance",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "OperationType",
                table: "BalanceTransaction",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "BalanceTransaction",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "BalanceTransaction",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UserBalance_UserId",
                table: "UserBalance",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceTransaction_UserBalance_UserId",
                table: "BalanceTransaction",
                column: "UserId",
                principalTable: "UserBalance",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceTransaction_UserBalance_UserId",
                table: "BalanceTransaction");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UserBalance_UserId",
                table: "UserBalance");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentAmount",
                table: "UserBalance",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "OperationType",
                table: "BalanceTransaction",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "BalanceTransaction",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "BalanceTransaction",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "UserBalanceId",
                table: "BalanceTransaction",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BalanceTransaction_UserBalanceId",
                table: "BalanceTransaction",
                column: "UserBalanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceTransaction_AspNetUsers_UserId",
                table: "BalanceTransaction",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceTransaction_UserBalance_UserBalanceId",
                table: "BalanceTransaction",
                column: "UserBalanceId",
                principalTable: "UserBalance",
                principalColumn: "Id");
        }
    }
}
