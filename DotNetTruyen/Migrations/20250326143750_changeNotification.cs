using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DotNetTruyen.Migrations
{
    /// <inheritdoc />
    public partial class changeNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("9857632b-1ee0-42e3-ad05-fff77d8b2ad4"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("c37524cb-a60c-41bb-97a0-caf9f37e537d"), new Guid("6d60f500-a4a6-4f64-8d9f-2773876ade87") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c37524cb-a60c-41bb-97a0-caf9f37e537d"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6d60f500-a4a6-4f64-8d9f-2773876ade87"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Notification",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Levels",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "ExpRequired", "LevelNumber", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("5608324c-9398-4063-a47b-fdef922173e2"), null, null, null, 0, 0, "Level 0", new DateTime(2025, 3, 26, 21, 37, 49, 93, DateTimeKind.Local).AddTicks(4204), null });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("35fae12d-7023-459f-a867-172422651b2e"), null, "Admin", "ADMIN" },
                    { new Guid("79724d32-f2ff-44be-9723-cef60d2bb12b"), null, "Reader", "READER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Exp", "ImageUrl", "LevelId", "LockoutEnabled", "LockoutEnd", "NameToDisplay", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("3f67619e-0d1f-4b99-ba46-bae1c4408445"), 0, "9766d098-8b46-43bd-829f-186141a14194", "admin@example.com", true, 0, null, null, false, null, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEJxuxeWMbawdkd8ZYlm/ntrdwTw7pFm1fFu+7I9ckVFgJb+3atCeqjegv2IKEXZfzQ==", null, false, "c62ff94b-dd67-4317-925d-bcd8a91f86ac", false, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("35fae12d-7023-459f-a867-172422651b2e"), new Guid("3f67619e-0d1f-4b99-ba46-bae1c4408445") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: new Guid("5608324c-9398-4063-a47b-fdef922173e2"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("79724d32-f2ff-44be-9723-cef60d2bb12b"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("35fae12d-7023-459f-a867-172422651b2e"), new Guid("3f67619e-0d1f-4b99-ba46-bae1c4408445") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("35fae12d-7023-459f-a867-172422651b2e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3f67619e-0d1f-4b99-ba46-bae1c4408445"));

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Notification");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("9857632b-1ee0-42e3-ad05-fff77d8b2ad4"), null, "Reader", "READER" },
                    { new Guid("c37524cb-a60c-41bb-97a0-caf9f37e537d"), null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Exp", "ImageUrl", "LevelId", "LockoutEnabled", "LockoutEnd", "NameToDisplay", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("6d60f500-a4a6-4f64-8d9f-2773876ade87"), 0, "3b2ea8df-bd34-41c1-bc50-0333090c03e5", "admin@example.com", true, 0, null, null, false, null, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEHSfYgqp7Uwgf/co+ueQ7s6OsLL1SXmCf40rho5Z4Id7o9B/5UTzMBFSoYLdoMi5Hw==", null, false, "15636e44-d4af-45ac-9c98-73d74414e129", false, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("c37524cb-a60c-41bb-97a0-caf9f37e537d"), new Guid("6d60f500-a4a6-4f64-8d9f-2773876ade87") });
        }
    }
}
