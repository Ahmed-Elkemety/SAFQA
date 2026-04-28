using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFQA.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ADDUserFollowersAuctionParticipationsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BidParticipations",
                columns: table => new
                {
                    AuctionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    PatoicipationTime = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidParticipations", x => new { x.AuctionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_BidParticipations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BidParticipations_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFollowers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    FollowedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowers", x => new { x.SellerId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserFollowers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollowers_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BidParticipations_UserId",
                table: "BidParticipations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowers_UserId_SellerId",
                table: "UserFollowers",
                columns: new[] { "UserId", "SellerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BidParticipations");

            migrationBuilder.DropTable(
                name: "UserFollowers");
        }
    }
}
