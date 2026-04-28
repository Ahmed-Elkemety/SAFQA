using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFQA.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnToAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEscrowReleased",
                table: "Auctions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEscrowReleased",
                table: "Auctions");
        }
    }
}
