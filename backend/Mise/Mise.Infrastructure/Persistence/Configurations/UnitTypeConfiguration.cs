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
    public class UnitTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<UnitType> builder)
        {
            builder.ToTable("unit_types");

            builder.HasKey(u => u.UnitTypeId);

            builder.Property(u => u.UnitTypeId)
                .HasColumnName("unit_type_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(u => u.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(u => u.Name)
                .HasColumnName("name")
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(u => u.Name)
                .IsUnique();

            builder.Property(u => u.Abbreviation)
                .HasColumnName("abbreviation")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(u => u.System)
                .HasColumnName("system")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(u => u.MeasureType)
                .HasColumnName("measure_type")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.ConversionFactor)
                .HasColumnName("conversion_factor")
                .HasPrecision(10, 6);

            builder.Property(u => u.IsNonConvertible)
                .HasColumnName("is_non_convertible")
                .HasDefaultValue(false);

            builder.Property(u => u.IsSystemDefined)
                .HasColumnName("is_system_defined")
                .HasDefaultValue(false);

            builder.HasIndex(u => new { u.TenantId, u.Name })
                .IsUnique();

            builder.HasOne(u => u.Tenant)
                .WithMany()
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
