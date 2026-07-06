using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixAllergenTagUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_allergen_tags_tenant_id",
                table: "allergen_tags");

            migrationBuilder.CreateIndex(
                name: "IX_allergen_tags_tenant_id_name",
                table: "allergen_tags",
                columns: new[] { "tenant_id", "name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_allergen_tags_tenant_id_name",
                table: "allergen_tags");

            migrationBuilder.CreateIndex(
                name: "IX_allergen_tags_tenant_id",
                table: "allergen_tags",
                column: "tenant_id",
                unique: true);
        }
    }
}
