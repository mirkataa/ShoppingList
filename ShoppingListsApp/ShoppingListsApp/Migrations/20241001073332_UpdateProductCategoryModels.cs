using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingListsApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductCategoryModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFile",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageFile",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageFile",
                table: "Products",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageFile",
                table: "Categories",
                type: "bytea",
                nullable: true);
        }
    }
}
