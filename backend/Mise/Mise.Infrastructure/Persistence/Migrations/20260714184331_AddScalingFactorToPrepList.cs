using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScalingFactorToPrepList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "scaling_factor",
                table: "prep_list_items",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 1m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "scaling_factor",
                table: "prep_list_items");
        }
    }
}
