using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileSynchornization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "623543b8-700f-4e03-b9f5-00aa5ac5ed02", null, "Gambler", "GAMBLER" },
                    { "9841d017-abda-45a2-86e0-6660b90d1ff7", null, "Administrator", "ADMINISTRATOR" },
                    { "992512b8-d568-4520-a05b-8c5fa2b2e5b0", null, "Moderator", "MODERATOR" },
                    { "a767fa6f-48cc-4b13-b794-75a2b6a6917b", null, "Bookmaker", "BOOKMAKER" },
                    { "d30880ee-82f5-40dc-bd55-c0db9f6a5f02", null, "Guest", "GUEST" },
                    { "d3ec25a8-4b31-4b04-8c18-1242d6fb86d0", null, "Banned", "BANNED" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "623543b8-700f-4e03-b9f5-00aa5ac5ed02");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9841d017-abda-45a2-86e0-6660b90d1ff7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "992512b8-d568-4520-a05b-8c5fa2b2e5b0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a767fa6f-48cc-4b13-b794-75a2b6a6917b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d30880ee-82f5-40dc-bd55-c0db9f6a5f02");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d3ec25a8-4b31-4b04-8c18-1242d6fb86d0");

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
        }
    }
}
