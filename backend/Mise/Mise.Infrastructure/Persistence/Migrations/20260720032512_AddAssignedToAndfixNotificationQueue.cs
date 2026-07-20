using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedToAndfixNotificationQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notification_queue_tenants_teantn_id",
                table: "notification_queue");

            migrationBuilder.RenameColumn(
                name: "teantn_id",
                table: "notification_queue",
                newName: "tenant_id");

            migrationBuilder.RenameIndex(
                name: "IX_notification_queue_teantn_id",
                table: "notification_queue",
                newName: "IX_notification_queue_tenant_id");

            migrationBuilder.AddColumn<Guid>(
                name: "assigned_to",
                table: "prep_lists",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_prep_lists_assigned_to",
                table: "prep_lists",
                column: "assigned_to");

            migrationBuilder.AddForeignKey(
                name: "FK_notification_queue_tenants_tenant_id",
                table: "notification_queue",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "tenant_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_prep_lists_users_assigned_to",
                table: "prep_lists",
                column: "assigned_to",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notification_queue_tenants_tenant_id",
                table: "notification_queue");

            migrationBuilder.DropForeignKey(
                name: "FK_prep_lists_users_assigned_to",
                table: "prep_lists");

            migrationBuilder.DropIndex(
                name: "IX_prep_lists_assigned_to",
                table: "prep_lists");

            migrationBuilder.DropColumn(
                name: "assigned_to",
                table: "prep_lists");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "notification_queue",
                newName: "teantn_id");

            migrationBuilder.RenameIndex(
                name: "IX_notification_queue_tenant_id",
                table: "notification_queue",
                newName: "IX_notification_queue_teantn_id");

            migrationBuilder.AddForeignKey(
                name: "FK_notification_queue_tenants_teantn_id",
                table: "notification_queue",
                column: "teantn_id",
                principalTable: "tenants",
                principalColumn: "tenant_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
