using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mise.Domain.Entities;

namespace Mise.Infrastructure.Persistence.Configurations
{
    public class MenuItemRecipeConfiguration : IEntityTypeConfiguration<MenuItemRecipe>
    {
        public void Configure(EntityTypeBuilder<MenuItemRecipe> builder)
        {
            builder.ToTable("menu_item_recipes");

            builder.HasKey(mir => mir.MenuItemRecipeId);

            builder.Property(mir => mir.MenuItemRecipeId)
                .HasColumnName("menu_item_recipe_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(mir => mir.MenuItemId)
                .HasColumnName("menu_item_id")
                .IsRequired();

            builder.Property(mir => mir.RecipeId)
                .HasColumnName("recipe_id")
                .IsRequired();

            builder.Property(mir => mir.DisplayOrder)
                .HasColumnName("display_order")
                .IsRequired();

            builder.Property(mir => mir.Note)
                .HasColumnName("note")
                .HasMaxLength(255);

            builder.HasIndex(mir => new { mir.MenuItemId, mir.RecipeId })
                .IsUnique();

            builder.HasOne(mir => mir.MenuItem)
                .WithMany(mi => mi.MenuItemRecipes)
                .HasForeignKey(mir => mir.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mir => mir.Recipe)
                .WithMany(r => r.MenuItemRecipes)
                .HasForeignKey(mir => mir.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
