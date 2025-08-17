using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppBanHang.Migrations
{
    /// <inheritdoc />
    public partial class Init7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Discounts",
                newName: "DiscountPercent");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Discounts");

            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "Discounts",
                newName: "Amount");
        }
    }
}
