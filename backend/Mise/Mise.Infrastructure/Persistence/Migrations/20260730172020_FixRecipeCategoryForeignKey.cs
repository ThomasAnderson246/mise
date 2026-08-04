using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixRecipeCategoryForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recipe_categories_categories_recipe_id",
                table: "recipe_categories");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_categories_category_id",
                table: "recipe_categories",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_recipe_categories_categories_category_id",
                table: "recipe_categories",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "category_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recipe_categories_categories_category_id",
                table: "recipe_categories");

            migrationBuilder.DropIndex(
                name: "IX_recipe_categories_category_id",
                table: "recipe_categories");

            migrationBuilder.AddForeignKey(
                name: "FK_recipe_categories_categories_recipe_id",
                table: "recipe_categories",
                column: "recipe_id",
                principalTable: "categories",
                principalColumn: "category_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
