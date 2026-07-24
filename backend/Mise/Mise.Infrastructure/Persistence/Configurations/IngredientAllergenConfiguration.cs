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
    public class IngredientAllergenConfiguration : IEntityTypeConfiguration<IngredientAllergen>
    {
        public void Configure(EntityTypeBuilder<IngredientAllergen> builder)
        {
            builder.ToTable("ingredient_allergens");

            builder.HasKey(ia => new {ia.IngredientId, ia.AllergenId});

            builder.Property(ia => ia.IngredientId)
                .HasColumnName("ingredient_id");

            builder.Property(ia => ia.AllergenId)
                .HasColumnName("allergen_id");

            builder.HasOne(ia => ia.Ingredient)
                .WithMany(i => i.IngredientAllergens)
                .HasForeignKey(ia => ia.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ia => ia.AllergenTag)
                .WithMany(a => a.IngredientAllergens)
                .HasForeignKey(ia => ia.AllergenId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
}
