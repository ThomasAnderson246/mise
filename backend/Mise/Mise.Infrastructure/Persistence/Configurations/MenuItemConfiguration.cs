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
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.ToTable("menu_items");

            builder.HasKey(mi => mi.MenuItemId);

            builder.Property(mi => mi.MenuItemId)
                .HasColumnName("menu_item_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(mi => mi.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(mi => mi.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(mi => mi.Description)
                .HasColumnName("description");

            builder.Property(mi => mi.Course)
                .HasColumnName("course")
                .HasMaxLength(50);

            builder.Property(mi => mi.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(mi => mi.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            builder.Property(mi => mi.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(mi => mi.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(mi => mi.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.HasIndex(mi => new { mi.TenantId, mi.Name })
                .IsUnique();

            builder.HasOne(mi => mi.Tenant)
                .WithMany(t => t.MenuItems)
                .HasForeignKey(mi => mi.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mi => mi.CreatedByUser)
                .WithMany()
                .HasForeignKey(mi => mi.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
