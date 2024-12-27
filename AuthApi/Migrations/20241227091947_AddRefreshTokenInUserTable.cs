using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenInUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "035e6613-5d0c-4f69-8138-1bfb6ed26e31");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "743f0d41-c880-4e68-ace4-a2d9b3ba8e8e");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "b13ad59d-57b7-4fef-b8e2-bf31e794ce21");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "244d6e20-8a11-41d8-9cd1-1d3b58fa6cf8", null, "Manager", "MANAGER" },
                    { "77d88e0c-6b9a-4b3b-a509-66499f5caa95", null, "Admin", "ADMIN" },
                    { "91eb0d24-7441-4033-b07d-c36b689bd8e6", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "244d6e20-8a11-41d8-9cd1-1d3b58fa6cf8");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "77d88e0c-6b9a-4b3b-a509-66499f5caa95");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "91eb0d24-7441-4033-b07d-c36b689bd8e6");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "035e6613-5d0c-4f69-8138-1bfb6ed26e31", null, "Admin", "ADMIN" },
                    { "743f0d41-c880-4e68-ace4-a2d9b3ba8e8e", null, "User", "USER" },
                    { "b13ad59d-57b7-4fef-b8e2-bf31e794ce21", null, "Manager", "MANAGER" }
                });
        }
    }
}
