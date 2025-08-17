using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppBanHang.Migrations
{
    /// <inheritdoc />
    public partial class Init8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Discounts",
                newName: "DiscountDescription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountDescription",
                table: "Discounts",
                newName: "Description");
        }
    }
}
