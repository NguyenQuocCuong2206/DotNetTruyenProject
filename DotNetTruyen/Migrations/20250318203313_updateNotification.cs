using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DotNetTruyen.Migrations
{
    /// <inheritdoc />
    public partial class updateNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("eb22ca27-8e16-4ee9-ac45-35b5d9188691"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("b5635608-6ba4-4900-b3ab-528960bbbe48"), new Guid("369c6cf6-5bea-4580-b836-2f58c9c254f4") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b5635608-6ba4-4900-b3ab-528960bbbe48"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("369c6cf6-5bea-4580-b836-2f58c9c254f4"));

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Chapters");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("4504cfc0-a76d-4dfa-9d1a-4f70c1ace6a2"), null, "Admin", "ADMIN" },
                    { new Guid("c772c005-a600-497f-b117-15887efb73b5"), null, "Reader", "READER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "ImageUrl", "LockoutEnabled", "LockoutEnd", "NameToDisplay", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("a655e1a9-3814-476d-a386-96755dfdf2f2"), 0, "d5bca121-fcbd-4128-a4fa-858dbf3f52aa", "admin@example.com", true, null, false, null, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEIzPETvAeXvTfCbRg/LBCtzBZyGOL3H1usWx8852dz2lDkGkteeSnT7DWZ/ZDvY29g==", null, false, "0fee338e-c1e5-487c-8d9c-a296579e4f0a", false, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("4504cfc0-a76d-4dfa-9d1a-4f70c1ace6a2"), new Guid("a655e1a9-3814-476d-a386-96755dfdf2f2") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c772c005-a600-497f-b117-15887efb73b5"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("4504cfc0-a76d-4dfa-9d1a-4f70c1ace6a2"), new Guid("a655e1a9-3814-476d-a386-96755dfdf2f2") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4504cfc0-a76d-4dfa-9d1a-4f70c1ace6a2"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a655e1a9-3814-476d-a386-96755dfdf2f2"));

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Chapters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("b5635608-6ba4-4900-b3ab-528960bbbe48"), null, "Admin", "ADMIN" },
                    { new Guid("eb22ca27-8e16-4ee9-ac45-35b5d9188691"), null, "Reader", "READER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "ImageUrl", "LockoutEnabled", "LockoutEnd", "NameToDisplay", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("369c6cf6-5bea-4580-b836-2f58c9c254f4"), 0, "680dc20e-e56c-4e2a-9d44-3451960462ef", "admin@example.com", true, null, false, null, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEH7l98quGHNys/ZBmC09lIQGfGjfncKc5iV/xoPJZXGjDqGLWyHeUnQyH1z4rpUpLQ==", null, false, "0ae2df89-e8a4-4c1f-abd0-9815842360e2", false, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("b5635608-6ba4-4900-b3ab-528960bbbe48"), new Guid("369c6cf6-5bea-4580-b836-2f58c9c254f4") });
        }
    }
}
