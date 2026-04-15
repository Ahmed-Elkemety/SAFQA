using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFQA.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Updatedecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SecurityDeposit",
                table: "Auctions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 100.50m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalPrice",
                table: "Auctions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 100.50m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SecurityDeposit",
                table: "Auctions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 100.50m);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalPrice",
                table: "Auctions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 100.50m);
        }
    }
}
