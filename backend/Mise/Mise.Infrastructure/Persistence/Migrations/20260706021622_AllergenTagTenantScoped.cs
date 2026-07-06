using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AllergenTagTenantScoped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "copmleted_at",
                table: "prep_lists",
                newName: "completed_at");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "cooking_sessions",
                newName: "started_at");

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemDefined",
                table: "allergen_tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "allergen_tags",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_allergen_tags_tenant_id",
                table: "allergen_tags",
                column: "tenant_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_allergen_tags_tenants_tenant_id",
                table: "allergen_tags",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "tenant_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_allergen_tags_tenants_tenant_id",
                table: "allergen_tags");

            migrationBuilder.DropIndex(
                name: "IX_allergen_tags_tenant_id",
                table: "allergen_tags");

            migrationBuilder.DropColumn(
                name: "IsSystemDefined",
                table: "allergen_tags");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "allergen_tags");

            migrationBuilder.RenameColumn(
                name: "completed_at",
                table: "prep_lists",
                newName: "copmleted_at");

            migrationBuilder.RenameColumn(
                name: "started_at",
                table: "cooking_sessions",
                newName: "StartedAt");
        }
    }
}
