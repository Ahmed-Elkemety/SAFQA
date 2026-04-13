using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFQA.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SetDefult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WinnerUserId",
                table: "Auctions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: " ",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WinnerUserId",
                table: "Auctions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "0",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: " ");
        }
    }
}
