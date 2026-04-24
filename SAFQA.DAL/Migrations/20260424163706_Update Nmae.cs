using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFQA.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNmae : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BidParticipations_AspNetUsers_UserId",
                table: "BidParticipations");

            migrationBuilder.DropForeignKey(
                name: "FK_BidParticipations_Auctions_AuctionId",
                table: "BidParticipations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BidParticipations",
                table: "BidParticipations");

            migrationBuilder.RenameTable(
                name: "BidParticipations",
                newName: "auctionParticipations");

            migrationBuilder.RenameIndex(
                name: "IX_BidParticipations_UserId",
                table: "auctionParticipations",
                newName: "IX_auctionParticipations_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_auctionParticipations",
                table: "auctionParticipations",
                columns: new[] { "AuctionId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_auctionParticipations_AspNetUsers_UserId",
                table: "auctionParticipations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_auctionParticipations_Auctions_AuctionId",
                table: "auctionParticipations",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_auctionParticipations_AspNetUsers_UserId",
                table: "auctionParticipations");

            migrationBuilder.DropForeignKey(
                name: "FK_auctionParticipations_Auctions_AuctionId",
                table: "auctionParticipations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_auctionParticipations",
                table: "auctionParticipations");

            migrationBuilder.RenameTable(
                name: "auctionParticipations",
                newName: "BidParticipations");

            migrationBuilder.RenameIndex(
                name: "IX_auctionParticipations_UserId",
                table: "BidParticipations",
                newName: "IX_BidParticipations_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BidParticipations",
                table: "BidParticipations",
                columns: new[] { "AuctionId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BidParticipations_AspNetUsers_UserId",
                table: "BidParticipations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BidParticipations_Auctions_AuctionId",
                table: "BidParticipations",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
