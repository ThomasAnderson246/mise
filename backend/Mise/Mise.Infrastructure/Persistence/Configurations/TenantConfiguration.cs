using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("tenants");

            builder.HasKey(t => t.TenantId);

            builder.Property(t => t.TenantId)
                .HasColumnName("tenant_id")
                .HasDefaultValueSql("gen_random_uuid()");
            builder.Property(t => t.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(t => t.Slug)
                .HasColumnName("slug")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(t => t.Slug)
                .IsUnique();

            builder.Property(t => t.LogoUrl)
                .HasColumnName("logo_url");

            builder.Property(t => t.PrimaryColour)
                .HasColumnName("primary_colour")
                .HasMaxLength(7);
            
            builder.Property(t => t.SecondaryColour)
                .HasColumnName("secondary_colour")
                .HasMaxLength(7);

            builder.Property(t => t.DefaultUnitSystem)
                .HasColumnName("default_unit_system")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(t => t.Tier)
                .HasColumnName("tier")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(t => t.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();
        }
    }
}
