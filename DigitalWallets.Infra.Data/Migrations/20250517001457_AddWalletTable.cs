using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalWallets.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Wallets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Wallets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Wallets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Wallets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string[]>(
                name: "Roles",
                table: "AuthUser",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "AuthUser",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId1",
                table: "Wallets",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_AuthUser_UserId1",
                table: "Wallets",
                column: "UserId1",
                principalTable: "AuthUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_AuthUser_UserId1",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId1",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Roles",
                table: "AuthUser");

            migrationBuilder.DropColumn(
                name: "Success",
                table: "AuthUser");
        }
    }
}
