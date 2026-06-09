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
    public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
    {
        public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
        {
            builder.ToTable("recipe_ingredients");

            builder.HasKey(RecipeIngredient => RecipeIngredient.RecipeIngredientId);

            builder.Property(ri => ri.RecipeIngredientId)
                .HasColumnName("recipe_ingredient_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(ri => ri.VersionId)
                .HasColumnName("version_id")
                .IsRequired();

            builder.Property(ri => ri.IngredientId)
                .HasColumnName("ingredient_id")
                .IsRequired();

            builder.Property(ri => ri.Quantity)
                .HasColumnName("quantity")
                .HasPrecision(10, 4)
                .IsRequired();

            builder.Property(ri => ri.UnitTypeId)
                .HasColumnName("unit_type_id");

            builder.Property(ri => ri.IsNonConvertible)
                .HasColumnName("is_non_converitble")
                .HasDefaultValue(false);

            builder.Property(ri => ri.IsRatioAnchor)
                .HasColumnName("is_ratio_anchor")
                .HasDefaultValue(false);

            builder.Property(ri => ri.DisplayOrder)
                .HasColumnName("display_order")
                .IsRequired();

            builder.HasOne(ri => ri.Version)
                .WithMany(rv => rv.Ingredients)
                .HasForeignKey(ri => ri.VersionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ri => ri.UnitType)
                .WithMany(u => u.RecipeIngredients)
                .HasForeignKey(ri => ri.UnitTypeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
