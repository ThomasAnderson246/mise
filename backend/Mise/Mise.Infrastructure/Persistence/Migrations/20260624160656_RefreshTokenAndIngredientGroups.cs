using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefreshTokenAndIngredientGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "group_id",
                table: "recipe_ingredients",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "recipe_ingredient_groups",
                columns: table => new
                {
                    group_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ingredient_groups", x => x.group_id);
                    table.ForeignKey(
                        name: "FK_recipe_ingredient_groups_recipe_versions_version_id",
                        column: x => x.version_id,
                        principalTable: "recipe_versions",
                        principalColumn: "version_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    refresh_token_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.refresh_token_id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredients_group_id",
                table: "recipe_ingredients",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredient_groups_version_id",
                table: "recipe_ingredient_groups",
                column: "version_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_tenant_id",
                table: "refresh_tokens",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_recipe_ingredients_recipe_ingredient_groups_group_id",
                table: "recipe_ingredients",
                column: "group_id",
                principalTable: "recipe_ingredient_groups",
                principalColumn: "group_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.Sql("ALTER TABLE refresh_tokens ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE recipe_ingredient_groups ENABLE ROW LEVEL SECURITY;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recipe_ingredients_recipe_ingredient_groups_group_id",
                table: "recipe_ingredients");

            migrationBuilder.DropTable(
                name: "recipe_ingredient_groups");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_recipe_ingredients_group_id",
                table: "recipe_ingredients");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "recipe_ingredients");
        }
    }
}
