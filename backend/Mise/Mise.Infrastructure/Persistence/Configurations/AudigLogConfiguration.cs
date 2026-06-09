using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Mise.Domain.Entities;

namespace Mise.Infrastructure.Persistence.Configurations
{
    public class AudigLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("audit_logs");

            builder.HasKey(al => al.AuditLogId);

            builder.Property(al => al.AuditLogId)
                .HasColumnName("audit_log")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(al => al.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(al => al.PerformedBy)
                .HasColumnName("performed_by");

            builder.Property(al => al.Action)
                .HasColumnName("action")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(al => al.Resource)
                .HasColumnName("resource")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(al => al.ResourceId)
                .HasColumnName("resource_id")
                .IsRequired();

            builder.Property(al => al.PreviousState)
                .HasColumnName("previous_state")
                .HasColumnType("jsonb");

            builder.Property(al => al.NewState)
                .HasColumnName("new_state")
                .HasColumnType("jsonb");

            builder.Property(al => al.IpAddress)
                .HasColumnName("ip_address")
                .HasMaxLength(25);

            builder.Property(al => al.PerformedAt)
                .HasColumnName("performed_at")
                .IsRequired();

            builder.HasOne(al => al.Tenant)
                .WithMany(t => t.AuditLogs)
                .HasForeignKey(al => al.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(al => al.PerformedByUser)
                .WithMany()
                .HasForeignKey(al => al.PerformedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
