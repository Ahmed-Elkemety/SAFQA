using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFQA.DAL.Migrations
{
    /// <inheritdoc />
    using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

    public partial class df : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeletedAt",
                table: "Auctions",
                type: "nvarchar(max)",
                nullable: true,          // خلي العمود nullable
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeletedAt",
                table: "Auctions",
                type: "nvarchar(max)",
                nullable: false,         // رجعه غير nullable لو عملت rollback
                defaultValue: "",        // قيمة افتراضية لتجنب مشاكل البيانات القديمة
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
