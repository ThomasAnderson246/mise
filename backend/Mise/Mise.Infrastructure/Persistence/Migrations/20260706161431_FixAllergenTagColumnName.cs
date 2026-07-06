using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixAllergenTagColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSystemDefined",
                table: "allergen_tags",
                newName: "is_system_defined");

            migrationBuilder.AlterColumn<bool>(
                name: "is_system_defined",
                table: "allergen_tags",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_system_defined",
                table: "allergen_tags",
                newName: "IsSystemDefined");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSystemDefined",
                table: "allergen_tags",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
