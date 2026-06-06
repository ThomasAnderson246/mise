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
    public class PermisisonConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("permissions");

            builder.HasKey(p => p.PermissionId);

            builder.Property(p => p.PermissionId)
                .HasColumnName("permission_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(p => p.Name)
                .IsUnique();

            builder.Property(p => p.Description)
                .HasColumnName("description");

            builder.Property(p => p.Resource)
                .HasColumnName("resource")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Action)
                .HasColumnName("action")
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
