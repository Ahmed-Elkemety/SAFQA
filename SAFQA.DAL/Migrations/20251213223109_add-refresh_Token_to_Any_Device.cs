using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFQA.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addrefresh_Token_to_Any_Device : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "refreshTokens");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "refreshTokens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "refreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TokenHash",
                table: "refreshTokens",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "refreshTokens");

            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "refreshTokens");

            migrationBuilder.DropColumn(
                name: "TokenHash",
                table: "refreshTokens");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "refreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
