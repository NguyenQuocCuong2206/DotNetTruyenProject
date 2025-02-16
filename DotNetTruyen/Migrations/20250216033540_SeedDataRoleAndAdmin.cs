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
            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Likes");

            migrationBuilder.AlterColumn<string>(
                name: "UserIpHash",
                table: "Likes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                columns: new[] { "UserIpHash", "ComicId" });

            

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a2ca42aa-be5a-4169-bacd-4c3e8a8b567a", null, "Reader", "READER" },
                    { "a5e8c0d6-9b7c-41ca-a8a3-21a997fe2ae1", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id","FirstName","LastName","DateOfBirth", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e52c0bc6-ba54-42b4-877b-166090d0e6e0","Nguyen","Admin","09-09-1999", 0, "9c76cbdc-9434-463d-88bb-af4ac90f7613", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAIAAYagAAAAECtlwQEhbWgGkfSLMMqm07AZ581JUuyZzKUBRKsF8t7iWaeOyELp5AgyLfxZz2uzxw==", null, false, "57f56c14-f2cd-465c-be29-bd6ba0862543", false, "admin@example.com" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a5e8c0d6-9b7c-41ca-a8a3-21a997fe2ae1", "e52c0bc6-ba54-42b4-877b-166090d0e6e0" });
        }

        
    }
}
