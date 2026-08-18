using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PrepListRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prep_list_items_recipes_recipe_id",
                table: "prep_list_items");

            migrationBuilder.AlterColumn<decimal>(
                name: "scaling_factor",
                table: "prep_list_items",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldDefaultValue: 1m);

            migrationBuilder.AddColumn<Guid>(
                name: "anchor_ingredient_id",
                table: "prep_list_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "anchor_quantity",
                table: "prep_list_items",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "item_name",
                table: "prep_list_items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "prep_list_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "quantity",
                table: "prep_list_items",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source_type",
                table: "prep_list_items",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "unit",
                table: "prep_list_items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_prep_list_items_anchor_ingredient_id",
                table: "prep_list_items",
                column: "anchor_ingredient_id");

            migrationBuilder.AddForeignKey(
                name: "FK_prep_list_items_ingredients_anchor_ingredient_id",
                table: "prep_list_items",
                column: "anchor_ingredient_id",
                principalTable: "ingredients",
                principalColumn: "ingredient_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_prep_list_items_recipes_recipe_id",
                table: "prep_list_items",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prep_list_items_ingredients_anchor_ingredient_id",
                table: "prep_list_items");

            migrationBuilder.DropForeignKey(
                name: "FK_prep_list_items_recipes_recipe_id",
                table: "prep_list_items");

            migrationBuilder.DropIndex(
                name: "IX_prep_list_items_anchor_ingredient_id",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "anchor_ingredient_id",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "anchor_quantity",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "item_name",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "source_type",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "unit",
                table: "prep_list_items");

            migrationBuilder.AlterColumn<decimal>(
                name: "scaling_factor",
                table: "prep_list_items",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,4)",
                oldPrecision: 10,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_prep_list_items_recipes_recipe_id",
                table: "prep_list_items",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
