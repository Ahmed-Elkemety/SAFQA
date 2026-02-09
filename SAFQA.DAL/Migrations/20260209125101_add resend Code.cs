using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFQA.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addresendCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PasswordResetOtps",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Attempts",
                table: "PasswordResetOtps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PasswordResetOtps",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetOtps_UserId",
                table: "PasswordResetOtps",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResetOtps_AspNetUsers_UserId",
                table: "PasswordResetOtps",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResetOtps_AspNetUsers_UserId",
                table: "PasswordResetOtps");

            migrationBuilder.DropIndex(
                name: "IX_PasswordResetOtps_UserId",
                table: "PasswordResetOtps");

            migrationBuilder.DropColumn(
                name: "Attempts",
                table: "PasswordResetOtps");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PasswordResetOtps");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PasswordResetOtps",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
