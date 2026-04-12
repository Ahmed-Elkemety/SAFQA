using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFQA.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EditSavedCardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SavedCards_WalletId_PaymentToken",
                table: "SavedCards");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "SavedCards");

            migrationBuilder.DropColumn(
                name: "PaymentToken",
                table: "SavedCards");

            migrationBuilder.AddColumn<string>(
                name: "CardholderName",
                table: "SavedCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCards_WalletId",
                table: "SavedCards",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SavedCards_WalletId",
                table: "SavedCards");

            migrationBuilder.DropColumn(
                name: "CardholderName",
                table: "SavedCards");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "SavedCards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentToken",
                table: "SavedCards",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCards_WalletId_PaymentToken",
                table: "SavedCards",
                columns: new[] { "WalletId", "PaymentToken" },
                unique: true);
        }
    }
}
