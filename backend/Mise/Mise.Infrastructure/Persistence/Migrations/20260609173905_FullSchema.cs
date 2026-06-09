using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FullSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "allergen_tags",
                columns: table => new
                {
                    allergen_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_major = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allergen_tags", x => x.allergen_id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    resource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    action = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.permission_id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    logo_url = table.Column<string>(type: "text", nullable: true),
                    primary_colour = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    secondary_colour = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    tier = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    default_unit_system = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.tenant_id);
                });

            migrationBuilder.CreateTable(
                name: "UnitTypes",
                columns: table => new
                {
                    UnitTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Abbreviation = table.Column<string>(type: "text", nullable: false),
                    System = table.Column<string>(type: "text", nullable: false),
                    MeasureType = table.Column<string>(type: "text", nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "numeric", nullable: true),
                    IsNonConvertible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTypes", x => x.UnitTypeId);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.category);
                    table.ForeignKey(
                        name: "FK_categories_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_system_role = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                    table.ForeignKey(
                        name: "FK_roles_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    unit_preference = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "permission_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    audit_log = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    performed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    resource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    previous_state = table.Column<string>(type: "jsonb", nullable: true),
                    new_state = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    performed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.audit_log);
                    table.ForeignKey(
                        name: "FK_audit_logs_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_audit_logs_users_performed_by",
                        column: x => x.performed_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ingredients",
                columns: table => new
                {
                    ingredient_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    default_unit_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_non_convertible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredients", x => x.ingredient_id);
                    table.ForeignKey(
                        name: "FK_ingredients_UnitTypes_default_unit_type_id",
                        column: x => x.default_unit_type_id,
                        principalTable: "UnitTypes",
                        principalColumn: "UnitTypeId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ingredients_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ingredients_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "menu_items",
                columns: table => new
                {
                    menu_item_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    course = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_items", x => x.menu_item_id);
                    table.ForeignKey(
                        name: "FK_menu_items_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_menu_items_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_notifications_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notifications_users_recipient_id",
                        column: x => x.recipient_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prep_lists",
                columns: table => new
                {
                    prep_list_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    copmleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_complete = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prep_lists", x => x.prep_list_id);
                    table.ForeignKey(
                        name: "FK_prep_lists_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prep_lists_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_role_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    assigned_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.user_role_id);
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_assigned_by",
                        column: x => x.assigned_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ingredient_allergens",
                columns: table => new
                {
                    ingredient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alleren_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient_allergens", x => new { x.ingredient_id, x.alleren_id });
                    table.ForeignKey(
                        name: "FK_ingredient_allergens_allergen_tags_alleren_id",
                        column: x => x.alleren_id,
                        principalTable: "allergen_tags",
                        principalColumn: "allergen_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ingredient_allergens_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalTable: "ingredients",
                        principalColumn: "ingredient_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification_queue",
                columns: table => new
                {
                    queue_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    teantn_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    queued_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delivered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_delivered = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_queue", x => x.queue_id);
                    table.ForeignKey(
                        name: "FK_notification_queue_notifications_notification_id",
                        column: x => x.notification_id,
                        principalTable: "notifications",
                        principalColumn: "notification_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notification_queue_tenants_teantn_id",
                        column: x => x.teantn_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notification_queue_users_recipient_id",
                        column: x => x.recipient_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cooking_sessions",
                columns: table => new
                {
                    session_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_by = table.Column<Guid>(type: "uuid", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_complete = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cooking_sessions", x => x.session_id);
                    table.ForeignKey(
                        name: "FK_cooking_sessions_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cooking_sessions_users_started_by",
                        column: x => x.started_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ingredient_check_offs",
                columns: table => new
                {
                    check_off_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_ingredient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    checked_by = table.Column<Guid>(type: "uuid", nullable: true),
                    checked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_complete = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient_check_offs", x => x.check_off_id);
                    table.ForeignKey(
                        name: "FK_ingredient_check_offs_cooking_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "cooking_sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ingredient_check_offs_users_checked_by",
                        column: x => x.checked_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "menu_item_allergens",
                columns: table => new
                {
                    menu_item_allergen_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    menu_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allergen_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    source_recipe_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_component = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_direct = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_manual = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_item_allergens", x => x.menu_item_allergen_id);
                    table.ForeignKey(
                        name: "FK_menu_item_allergens_allergen_tags_allergen_id",
                        column: x => x.allergen_id,
                        principalTable: "allergen_tags",
                        principalColumn: "allergen_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_menu_item_allergens_menu_items_menu_item_id",
                        column: x => x.menu_item_id,
                        principalTable: "menu_items",
                        principalColumn: "menu_item_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "menu_item_recipes",
                columns: table => new
                {
                    menu_item_recipe_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    menu_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_item_recipes", x => x.menu_item_recipe_id);
                    table.ForeignKey(
                        name: "FK_menu_item_recipes_menu_items_menu_item_id",
                        column: x => x.menu_item_id,
                        principalTable: "menu_items",
                        principalColumn: "menu_item_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prep_list_items",
                columns: table => new
                {
                    prep_list_item_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    prep_list_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_complete = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    completed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prep_list_items", x => x.prep_list_item_id);
                    table.ForeignKey(
                        name: "FK_prep_list_items_prep_lists_prep_list_id",
                        column: x => x.prep_list_id,
                        principalTable: "prep_lists",
                        principalColumn: "prep_list_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prep_list_items_users_completed_by",
                        column: x => x.completed_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "recipe_categories",
                columns: table => new
                {
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_categories", x => new { x.recipe_id, x.category_id });
                    table.ForeignKey(
                        name: "FK_recipe_categories_categories_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "categories",
                        principalColumn: "category",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredients",
                columns: table => new
                {
                    recipe_ingredient_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ingredient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    unit_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_non_converitble = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_ratio_anchor = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ingredients", x => x.recipe_ingredient_id);
                    table.ForeignKey(
                        name: "FK_recipe_ingredients_UnitTypes_unit_type_id",
                        column: x => x.unit_type_id,
                        principalTable: "UnitTypes",
                        principalColumn: "UnitTypeId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_recipe_ingredients_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalTable: "ingredients",
                        principalColumn: "ingredient_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recipe_steps",
                columns: table => new
                {
                    step_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_number = table.Column<int>(type: "integer", nullable: false),
                    instruction = table.Column<string>(type: "text", nullable: false),
                    is_async = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    async_group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    has_timer = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    timer_duration = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_steps", x => x.step_id);
                });

            migrationBuilder.CreateTable(
                name: "step_check_offs",
                columns: table => new
                {
                    check_off_Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    checked_by = table.Column<Guid>(type: "uuid", nullable: true),
                    checked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_complete = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_step_check_offs", x => x.check_off_Id);
                    table.ForeignKey(
                        name: "FK_step_check_offs_cooking_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "cooking_sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_step_check_offs_recipe_steps_step_id",
                        column: x => x.step_id,
                        principalTable: "recipe_steps",
                        principalColumn: "step_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_step_check_offs_users_checked_by",
                        column: x => x.checked_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "recipe_versions",
                columns: table => new
                {
                    version_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_number = table.Column<int>(type: "integer", nullable: false),
                    is_draft = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_published = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    published_by = table.Column<Guid>(type: "uuid", nullable: true),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_versions", x => x.version_id);
                    table.ForeignKey(
                        name: "FK_recipe_versions_users_published_by",
                        column: x => x.published_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                columns: table => new
                {
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    scaling_mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    current_version_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipes", x => x.recipe_id);
                    table.ForeignKey(
                        name: "FK_recipes_recipe_versions_current_version_id",
                        column: x => x.current_version_id,
                        principalTable: "recipe_versions",
                        principalColumn: "version_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_recipes_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipes_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "sub_recipes",
                columns: table => new
                {
                    parent_recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sub_recipe_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sub_recipes", x => new { x.parent_recipe_id, x.sub_recipe_id });
                    table.CheckConstraint("chk_no_self_reference", "parent_recipe_id != sub_recipe_id");
                    table.ForeignKey(
                        name: "FK_sub_recipes_recipes_parent_recipe_id",
                        column: x => x.parent_recipe_id,
                        principalTable: "recipes",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sub_recipes_recipes_sub_recipe_id",
                        column: x => x.sub_recipe_id,
                        principalTable: "recipes",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transcription_jobs",
                columns: table => new
                {
                    job_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uploaded_by = table.Column<Guid>(type: "uuid", nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    generated_recipe_id = table.Column<Guid>(type: "uuid", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transcription_jobs", x => x.job_id);
                    table.ForeignKey(
                        name: "FK_transcription_jobs_recipes_generated_recipe_id",
                        column: x => x.generated_recipe_id,
                        principalTable: "recipes",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transcription_jobs_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transcription_jobs_users_uploaded_by",
                        column: x => x.uploaded_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "transcription_results",
                columns: table => new
                {
                    result_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    raw_response = table.Column<string>(type: "text", nullable: false),
                    parsed_title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    confidence_score = table.Column<decimal>(type: "numeric(4,3)", precision: 4, scale: 3, nullable: true),
                    flagged_fields = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transcription_results", x => x.result_id);
                    table.ForeignKey(
                        name: "FK_transcription_results_transcription_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "transcription_jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_allergen_tags_name",
                table: "allergen_tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_performed_by",
                table: "audit_logs",
                column: "performed_by");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_tenant_id",
                table: "audit_logs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_tenant_id_name",
                table: "categories",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cooking_sessions_recipe_id",
                table: "cooking_sessions",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_cooking_sessions_started_by",
                table: "cooking_sessions",
                column: "started_by");

            migrationBuilder.CreateIndex(
                name: "IX_cooking_sessions_tenant_id",
                table: "cooking_sessions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_cooking_sessions_version_id",
                table: "cooking_sessions",
                column: "version_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_allergens_alleren_id",
                table: "ingredient_allergens",
                column: "alleren_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_check_offs_checked_by",
                table: "ingredient_check_offs",
                column: "checked_by");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_check_offs_recipe_ingredient_id",
                table: "ingredient_check_offs",
                column: "recipe_ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_check_offs_session_id",
                table: "ingredient_check_offs",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_created_by",
                table: "ingredients",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_default_unit_type_id",
                table: "ingredients",
                column: "default_unit_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_tenant_id",
                table: "ingredients",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_allergens_allergen_id",
                table: "menu_item_allergens",
                column: "allergen_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_allergens_menu_item_id",
                table: "menu_item_allergens",
                column: "menu_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_allergens_source_recipe_id",
                table: "menu_item_allergens",
                column: "source_recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_recipes_menu_item_id_recipe_id",
                table: "menu_item_recipes",
                columns: new[] { "menu_item_id", "recipe_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_recipes_recipe_id",
                table: "menu_item_recipes",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_items_created_by",
                table: "menu_items",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_menu_items_tenant_id_name",
                table: "menu_items",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_queue_notification_id",
                table: "notification_queue",
                column: "notification_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_queue_recipient_id",
                table: "notification_queue",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_queue_teantn_id",
                table: "notification_queue",
                column: "teantn_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_recipient_id",
                table: "notifications",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_tenant_id",
                table: "notifications",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_name",
                table: "permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prep_list_items_completed_by",
                table: "prep_list_items",
                column: "completed_by");

            migrationBuilder.CreateIndex(
                name: "IX_prep_list_items_prep_list_id",
                table: "prep_list_items",
                column: "prep_list_id");

            migrationBuilder.CreateIndex(
                name: "IX_prep_list_items_recipe_id",
                table: "prep_list_items",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_prep_lists_created_by",
                table: "prep_lists",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_prep_lists_tenant_id",
                table: "prep_lists",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredients_ingredient_id",
                table: "recipe_ingredients",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredients_unit_type_id",
                table: "recipe_ingredients",
                column: "unit_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredients_version_id",
                table: "recipe_ingredients",
                column: "version_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_steps_version_id",
                table: "recipe_steps",
                column: "version_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_versions_published_by",
                table: "recipe_versions",
                column: "published_by");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_versions_recipe_id_version_number",
                table: "recipe_versions",
                columns: new[] { "recipe_id", "version_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipes_created_by",
                table: "recipes",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_current_version_id",
                table: "recipes",
                column: "current_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_tenant_id",
                table: "recipes",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_tenant_id_Name",
                table: "roles",
                columns: new[] { "tenant_id", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_step_check_offs_checked_by",
                table: "step_check_offs",
                column: "checked_by");

            migrationBuilder.CreateIndex(
                name: "IX_step_check_offs_session_id",
                table: "step_check_offs",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_step_check_offs_step_id",
                table: "step_check_offs",
                column: "step_id");

            migrationBuilder.CreateIndex(
                name: "IX_sub_recipes_sub_recipe_id",
                table: "sub_recipes",
                column: "sub_recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_slug",
                table: "tenants",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transcription_jobs_generated_recipe_id",
                table: "transcription_jobs",
                column: "generated_recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_transcription_jobs_tenant_id",
                table: "transcription_jobs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_transcription_jobs_uploaded_by",
                table: "transcription_jobs",
                column: "uploaded_by");

            migrationBuilder.CreateIndex(
                name: "IX_transcription_results_job_id",
                table: "transcription_results",
                column: "job_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_assigned_by",
                table: "user_roles",
                column: "assigned_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_user_id",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_tenant_id_email",
                table: "users",
                columns: new[] { "tenant_id", "email" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_cooking_sessions_recipe_versions_version_id",
                table: "cooking_sessions",
                column: "version_id",
                principalTable: "recipe_versions",
                principalColumn: "version_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cooking_sessions_recipes_recipe_id",
                table: "cooking_sessions",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ingredient_check_offs_recipe_ingredients_recipe_ingredient_~",
                table: "ingredient_check_offs",
                column: "recipe_ingredient_id",
                principalTable: "recipe_ingredients",
                principalColumn: "recipe_ingredient_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_menu_item_allergens_recipes_source_recipe_id",
                table: "menu_item_allergens",
                column: "source_recipe_id",
                principalTable: "recipes",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_menu_item_recipes_recipes_recipe_id",
                table: "menu_item_recipes",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_prep_list_items_recipes_recipe_id",
                table: "prep_list_items",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recipe_categories_recipes_recipe_id",
                table: "recipe_categories",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recipe_ingredients_recipe_versions_version_id",
                table: "recipe_ingredients",
                column: "version_id",
                principalTable: "recipe_versions",
                principalColumn: "version_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recipe_steps_recipe_versions_version_id",
                table: "recipe_steps",
                column: "version_id",
                principalTable: "recipe_versions",
                principalColumn: "version_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recipe_versions_recipes_recipe_id",
                table: "recipe_versions",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.Cascade);

            // Indexes
            migrationBuilder.Sql("CREATE INDEX idx_users_tenant ON users(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_roles_tenant ON roles(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_recipes_tenant ON recipes(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_ingredients_tenant ON ingredients(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_categories_tenant ON categories(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_prep_lists_tenant ON prep_lists(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_notifications_tenant ON notifications(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_audit_logs_tenant ON audit_logs(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_cooking_sessions_tenant ON cooking_sessions(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_recipes_status ON recipes(status);");
            migrationBuilder.Sql("CREATE INDEX idx_recipe_versions_recipe ON recipe_versions(recipe_id);");
            migrationBuilder.Sql("CREATE INDEX idx_recipe_steps_version ON recipe_steps(version_id);");
            migrationBuilder.Sql("CREATE INDEX idx_recipe_ingredients_version ON recipe_ingredients(version_id);");
            migrationBuilder.Sql("CREATE INDEX idx_notifications_recipient ON notifications(recipient_id);");
            migrationBuilder.Sql("CREATE INDEX idx_notification_queue_recipient ON notification_queue(recipient_id, is_delivered);");
            migrationBuilder.Sql("CREATE INDEX idx_audit_logs_resource ON audit_logs(resource, resource_id);");
            migrationBuilder.Sql("CREATE INDEX idx_audit_logs_performed_by ON audit_logs(performed_by);");
            migrationBuilder.Sql("CREATE INDEX idx_cooking_sessions_started_by ON cooking_sessions(started_by);");
            migrationBuilder.Sql("CREATE INDEX idx_prep_list_items_prep_list ON prep_list_items(prep_list_id);");
            migrationBuilder.Sql("CREATE INDEX idx_transcription_jobs_tenant ON transcription_jobs(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_menu_items_tenant ON menu_items(tenant_id);");
            migrationBuilder.Sql("CREATE INDEX idx_menu_items_status ON menu_items(status, is_active);");
            migrationBuilder.Sql("CREATE INDEX idx_menu_item_recipes_item ON menu_item_recipes(menu_item_id);");
            migrationBuilder.Sql("CREATE INDEX idx_menu_item_allergens_item ON menu_item_allergens(menu_item_id);");
            migrationBuilder.Sql("CREATE INDEX idx_menu_item_allergens_tag ON menu_item_allergens(allergen_id);");

            // Row Level Security
            migrationBuilder.Sql("ALTER TABLE users ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE roles ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE recipes ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE recipe_versions ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE recipe_steps ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE recipe_ingredients ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE ingredients ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE categories ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE prep_lists ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE prep_list_items ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE cooking_sessions ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE step_check_offs ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE ingredient_check_offs ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE notifications ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE notification_queue ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE audit_logs ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE transcription_jobs ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE transcription_results ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE menu_items ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE menu_item_recipes ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE menu_item_allergens ENABLE ROW LEVEL SECURITY;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recipes_tenants_tenant_id",
                table: "recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_users_tenants_tenant_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_recipe_versions_users_published_by",
                table: "recipe_versions");

            migrationBuilder.DropForeignKey(
                name: "FK_recipes_users_created_by",
                table: "recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_recipes_recipe_versions_current_version_id",
                table: "recipes");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "ingredient_allergens");

            migrationBuilder.DropTable(
                name: "ingredient_check_offs");

            migrationBuilder.DropTable(
                name: "menu_item_allergens");

            migrationBuilder.DropTable(
                name: "menu_item_recipes");

            migrationBuilder.DropTable(
                name: "notification_queue");

            migrationBuilder.DropTable(
                name: "prep_list_items");

            migrationBuilder.DropTable(
                name: "recipe_categories");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "step_check_offs");

            migrationBuilder.DropTable(
                name: "sub_recipes");

            migrationBuilder.DropTable(
                name: "transcription_results");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "recipe_ingredients");

            migrationBuilder.DropTable(
                name: "allergen_tags");

            migrationBuilder.DropTable(
                name: "menu_items");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "prep_lists");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "cooking_sessions");

            migrationBuilder.DropTable(
                name: "recipe_steps");

            migrationBuilder.DropTable(
                name: "transcription_jobs");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "ingredients");

            migrationBuilder.DropTable(
                name: "UnitTypes");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "recipe_versions");

            migrationBuilder.DropTable(
                name: "recipes");
        }
    }
}
