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
    public class MenuItemAllergenConfiguration : IEntityTypeConfiguration<MenuItemAllergen>
    {

        public void Configure(EntityTypeBuilder<MenuItemAllergen> builder)
        {
            builder.ToTable("menu_item_allergens");

            builder.HasKey(mia => mia.MenuItemAllergenId);

            builder.Property(mia => mia.MenuItemAllergenId)
                .HasColumnName("menu_item_allergen_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(mia => mia.MenuItemId)
                .HasColumnName("menu_item_id")
                .IsRequired();

            builder.Property(mia => mia.AllergenId)
                .HasColumnName("allergen_id")
                .IsRequired();

            builder.Property(mia => mia.SourceName)
                .HasColumnName("source_name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(mia => mia.SourceRecipeId)
                .HasColumnName("source_recipe_id");

            builder.Property(mia => mia.SourceComponent)
                .HasColumnName("source_component")
                .HasMaxLength(255);

            builder.Property(mia => mia.IsDirect)
                .HasColumnName("is_direct")
                .HasDefaultValue(false);

            builder.Property(mia => mia.IsManual)
                .HasColumnName("is_manual")
                .HasDefaultValue(false);

            builder.Property(mia => mia.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.HasOne(mia => mia.MenuItem)
                .WithMany(mi => mi.MenuItemAllergens)
                .HasForeignKey(mia => mia.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mia => mia.AllergenTag)
                .WithMany(a => a.MenuItemAllergens)
                .HasForeignKey(mia => mia.AllergenId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mia => mia.SourceRecipe)
                .WithMany()
                .HasForeignKey(mia => mia.SourceRecipeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
