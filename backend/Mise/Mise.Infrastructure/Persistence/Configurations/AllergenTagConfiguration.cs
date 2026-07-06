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
    public class AllergenTagConfiguration : IEntityTypeConfiguration<AllergenTag>
    {
        public void Configure(EntityTypeBuilder<AllergenTag> builder)
        {
            builder.ToTable("allergen_tags");

            builder.HasKey(a => a.AllergenId);

            builder.Property(a => a.AllergenId)
                .HasColumnName("allergen_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(a => a.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.HasIndex(a => new { a.TenantId, a.Name })
                .IsUnique();

            builder.Property(a => a.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(a => a.Name)
                .IsUnique();

            builder.Property(a => a.Description)
                .HasColumnName("description");

            builder.Property(a => a.IsMajor)
                .HasColumnName("is_major")
                .HasDefaultValue(false);

            builder.Property(a => a.IsSystemDefined)
                .HasColumnName("is_system_defined")
                .HasDefaultValue(false);

            builder.HasOne(a => a.Tenant)
                .WithMany(t => t.AllergenTags)
                .HasForeignKey(a => a.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
