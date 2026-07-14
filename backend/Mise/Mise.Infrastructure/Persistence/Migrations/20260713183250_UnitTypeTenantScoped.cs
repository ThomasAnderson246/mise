using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UnitTypeTenantScoped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystemDefined",
                table: "UnitTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "UnitTypes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UnitTypes_TenantId",
                table: "UnitTypes",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitTypes_tenants_TenantId",
                table: "UnitTypes",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "tenant_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitTypes_tenants_TenantId",
                table: "UnitTypes");

            migrationBuilder.DropIndex(
                name: "IX_UnitTypes_TenantId",
                table: "UnitTypes");

            migrationBuilder.DropColumn(
                name: "IsSystemDefined",
                table: "UnitTypes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UnitTypes");
        }
    }
}
