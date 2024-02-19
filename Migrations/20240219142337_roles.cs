using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthAPI.Migrations
{
    public partial class roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "03ef78d4-7d1c-4b2d-a59e-4f24e37edc8c", "b6151b1c-6a8b-490a-87d9-efee46f252dc", "Admin", "ADMİN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "49828c1f-5977-4ce0-916e-4dee2c80ceac", "2ec9fe40-dfbd-40f8-801c-072e58acb574", "Sales Advisor", "SALES ADVİSOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a86d3bcc-7229-4a86-a64b-977491dca486", "b04f8e9f-4123-4ea4-9311-4710a2018e1d", "Sales Manager", "SALES MANAGER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "03ef78d4-7d1c-4b2d-a59e-4f24e37edc8c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "49828c1f-5977-4ce0-916e-4dee2c80ceac");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a86d3bcc-7229-4a86-a64b-977491dca486");
        }
    }
}
