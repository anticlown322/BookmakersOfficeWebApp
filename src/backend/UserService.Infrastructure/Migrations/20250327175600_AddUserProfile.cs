using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "89aa0dbd-c511-428d-8a31-a1cc6f035070");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9d32c1e2-69cd-4651-ad41-41190c2a42aa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a19b6f9b-59b4-4e18-ade7-6265bae1735f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "acd3a917-d6c0-4526-9fc1-3a42f5289ee9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d6296c70-a0f5-49fb-b82f-5727ea43d2ab");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e33cb3fa-c362-4dac-986f-a0c4626f6191");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Roles = table.Column<string>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfile_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "45a5891d-80a2-48a6-8df6-1206b2173929", null, "Banned", "BANNED" },
                    { "805e770e-5f0d-4778-82e4-21b272eb77d6", null, "Administrator", "ADMINISTRATOR" },
                    { "9c9e9d0b-1509-4f10-a2b7-7df70310abe0", null, "Gambler", "GAMBLER" },
                    { "c17c4386-27e0-4bc1-8a2c-34e86e67c85c", null, "Bookmaker", "BOOKMAKER" },
                    { "e7c636b9-94a5-40ff-8b4a-a3f5285a949f", null, "Guest", "GUEST" },
                    { "f7b3fd27-1dcf-49ef-9676-9d169641f879", null, "Moderator", "MODERATOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_UserId",
                table: "UserProfile",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "45a5891d-80a2-48a6-8df6-1206b2173929");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "805e770e-5f0d-4778-82e4-21b272eb77d6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9c9e9d0b-1509-4f10-a2b7-7df70310abe0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c17c4386-27e0-4bc1-8a2c-34e86e67c85c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e7c636b9-94a5-40ff-8b4a-a3f5285a949f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f7b3fd27-1dcf-49ef-9676-9d169641f879");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "89aa0dbd-c511-428d-8a31-a1cc6f035070", null, "Administrator", "ADMINISTRATOR" },
                    { "9d32c1e2-69cd-4651-ad41-41190c2a42aa", null, "Moderator", "MODERATOR" },
                    { "a19b6f9b-59b4-4e18-ade7-6265bae1735f", null, "Bookmaker", "BOOKMAKER" },
                    { "acd3a917-d6c0-4526-9fc1-3a42f5289ee9", null, "Gambler", "GAMBLER" },
                    { "d6296c70-a0f5-49fb-b82f-5727ea43d2ab", null, "Guest", "GUEST" },
                    { "e33cb3fa-c362-4dac-986f-a0c4626f6191", null, "Banned", "BANNED" }
                });
        }
    }
}
