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
    public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder)
        {
            builder.ToTable("ingredients");

            builder.HasKey(i => i.IngredientId);

            builder.Property(i => i.IngredientId)
                .HasColumnName("ingredient_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(i => i.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(i => i.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(i => i.Category)
                .HasColumnName("category")
                .HasMaxLength(100);

            builder.Property(i => i.DefaultUnitTypeId)
                .HasColumnName("default_unit_type_id");

            builder.Property(i => i.IsNonConvertible)
                .HasColumnName("is_non_convertible")
                .HasDefaultValue(false);

            builder.Property(i => i.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired();

            builder.Property(i => i.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(i => i.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.HasOne(i => i.Tenant)
                .WithMany(t => t.Ingredients)
                .HasForeignKey(i => i.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.DefaultUnitType)
                .WithMany(u => u.Ingredients)
                .HasForeignKey(i => i.DefaultUnitTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(i => i.CreatedByUser)
                .WithMany()
                .HasForeignKey(i => i.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
   
}
