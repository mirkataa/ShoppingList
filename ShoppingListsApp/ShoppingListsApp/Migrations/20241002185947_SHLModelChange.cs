using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingListsApp.Migrations
{
    /// <inheritdoc />
    public partial class SHLModelChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ShoppingLists_ShoppingListModelShoppingListId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ShoppingListModelShoppingListId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShoppingListModelShoppingListId",
                table: "Products");

            migrationBuilder.AddColumn<List<string>>(
                name: "Items",
                table: "ShoppingLists",
                type: "text[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Items",
                table: "ShoppingLists");

            migrationBuilder.AddColumn<int>(
                name: "ShoppingListModelShoppingListId",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShoppingListModelShoppingListId",
                table: "Products",
                column: "ShoppingListModelShoppingListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ShoppingLists_ShoppingListModelShoppingListId",
                table: "Products",
                column: "ShoppingListModelShoppingListId",
                principalTable: "ShoppingLists",
                principalColumn: "ShoppingListId");
        }
    }
}
