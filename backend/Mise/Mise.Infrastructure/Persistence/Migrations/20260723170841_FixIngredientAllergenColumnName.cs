using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixIngredientAllergenColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ingredient_allergens_allergen_tags_alleren_id",
                table: "ingredient_allergens");

            migrationBuilder.RenameColumn(
                name: "alleren_id",
                table: "ingredient_allergens",
                newName: "allergen_id");

            migrationBuilder.RenameIndex(
                name: "IX_ingredient_allergens_alleren_id",
                table: "ingredient_allergens",
                newName: "IX_ingredient_allergens_allergen_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ingredient_allergens_allergen_tags_allergen_id",
                table: "ingredient_allergens",
                column: "allergen_id",
                principalTable: "allergen_tags",
                principalColumn: "allergen_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ingredient_allergens_allergen_tags_allergen_id",
                table: "ingredient_allergens");

            migrationBuilder.RenameColumn(
                name: "allergen_id",
                table: "ingredient_allergens",
                newName: "alleren_id");

            migrationBuilder.RenameIndex(
                name: "IX_ingredient_allergens_allergen_id",
                table: "ingredient_allergens",
                newName: "IX_ingredient_allergens_alleren_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ingredient_allergens_allergen_tags_alleren_id",
                table: "ingredient_allergens",
                column: "alleren_id",
                principalTable: "allergen_tags",
                principalColumn: "allergen_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
