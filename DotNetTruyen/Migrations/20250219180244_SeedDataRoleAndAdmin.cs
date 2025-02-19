using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DotNetTruyen.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataRoleAndAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4d320af3-6cd9-4153-bd28-51a92f98d40a", null, "Admin", "ADMIN" },
                    { "c35916bb-0934-4b7b-a1d4-24b564289e76", null, "Reader", "READER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id","NameToDisplay" ,"AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0a62238f-a18a-4acc-85d1-4891045ad6f7","Admin", 0, "2af26904-a474-4591-a6f7-c085eacf6eb9", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEMZLjuk3x8Gg+67zcx485E6os1Qyc8L3Zdu6kH7kmwr0JYGSyQjdnVAdAG5fAItEtQ==", null, false, "274403dc-4e3a-42cc-bfd7-6ed56ff62f39", false, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "4d320af3-6cd9-4153-bd28-51a92f98d40a", "0a62238f-a18a-4acc-85d1-4891045ad6f7" });
        }

    }
}
