using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orko.Portal.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFieldsToApiLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "ApiLogs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ApiLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ApiLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserRole",
                table: "ApiLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "ApiLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ApiLogs");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ApiLogs");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "ApiLogs");
        }
    }
}
