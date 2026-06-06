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
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.ToTable("recipes");

            builder.HasKey(r => r.RecipeId);

            builder.Property(r => r.RecipeId)
                .HasColumnName("recipe_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(r => r.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(r => r.Title)
                .HasColumnName("title")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(r => r.CurrentVersionId)
                .HasColumnName("current_version_id");

            builder.Property(r => r.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(r => r.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(r => r.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.HasOne(r => r.Tenant)
                .WithMany(t => t.Recipes)
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.CreatedByUser)
                .WithMany()
                .HasForeignKey(r => r.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne( r => r.CurrentVersion)
                .WithMany()
                .HasForeignKey(r => r.CurrentVersionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
