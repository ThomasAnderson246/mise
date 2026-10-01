using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityItemsAndIncompleteTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "incomplete_flagged_by",
                table: "prep_list_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "incomplete_note",
                table: "prep_list_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "incomplete_reason_code",
                table: "prep_list_items",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "incompleted_flagged_at",
                table: "prep_list_items",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_incomplete",
                table: "prep_list_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "priority_items",
                columns: table => new
                {
                    priority_item_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: true),
                    item_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    scaling_factor = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    anchor_ingredient_id = table.Column<Guid>(type: "uuid", nullable: true),
                    anchor_quantity = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    origin = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reason_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    reason_note = table.Column<string>(type: "text", nullable: true),
                    flagged_by = table.Column<Guid>(type: "uuid", nullable: false),
                    source_prep_list_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_prep_list_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priority_items", x => x.priority_item_id);
                    table.ForeignKey(
                        name: "FK_priority_items_ingredients_anchor_ingredient_id",
                        column: x => x.anchor_ingredient_id,
                        principalTable: "ingredients",
                        principalColumn: "ingredient_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_priority_items_prep_list_items_source_prep_list_item_id",
                        column: x => x.source_prep_list_item_id,
                        principalTable: "prep_list_items",
                        principalColumn: "prep_list_item_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_priority_items_prep_lists_source_prep_list_id",
                        column: x => x.source_prep_list_id,
                        principalTable: "prep_lists",
                        principalColumn: "prep_list_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_priority_items_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_priority_items_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_priority_items_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_priority_items_users_flagged_by",
                        column: x => x.flagged_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prep_list_items_incomplete_flagged_by",
                table: "prep_list_items",
                column: "incomplete_flagged_by");

            migrationBuilder.CreateIndex(
                name: "IX_priority_items_anchor_ingredient_id",
                table: "priority_items",
                column: "anchor_ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_priority_items_created_by",
                table: "priority_items",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_priority_items_flagged_by",
                table: "priority_items",
                column: "flagged_by");

            migrationBuilder.CreateIndex(
                name: "IX_priority_items_recipe_id",
                table: "priority_items",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_priority_items_source_prep_list_id",
                table: "priority_items",
                column: "source_prep_list_id");

            migrationBuilder.CreateIndex(
                name: "IX_priority_items_source_prep_list_item_id",
                table: "priority_items",
                column: "source_prep_list_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_priority_items_tenant_id",
                table: "priority_items",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "FK_prep_list_items_users_incomplete_flagged_by",
                table: "prep_list_items",
                column: "incomplete_flagged_by",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prep_list_items_users_incomplete_flagged_by",
                table: "prep_list_items");

            migrationBuilder.DropTable(
                name: "priority_items");

            migrationBuilder.DropIndex(
                name: "IX_prep_list_items_incomplete_flagged_by",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "incomplete_flagged_by",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "incomplete_note",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "incomplete_reason_code",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "incompleted_flagged_at",
                table: "prep_list_items");

            migrationBuilder.DropColumn(
                name: "is_incomplete",
                table: "prep_list_items");
        }
    }
}
