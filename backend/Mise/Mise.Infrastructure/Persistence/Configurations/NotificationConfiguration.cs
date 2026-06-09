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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");

            builder.HasKey(n => n.NotificationId);

            builder.Property(n => n.NotificationId)
                .HasColumnName("notification_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(n => n.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(n => n.RecipientId)
                .HasColumnName("recipient_id")
                .IsRequired();

            builder.Property(n => n.Title)
                .HasColumnName("title")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(n => n.Message)
                .HasColumnName("message")
                .IsRequired();

            builder.Property(n => n.Type)
                .HasColumnName("type")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(n => n.IsRead)
                .HasColumnName("is_read")
                .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(n => n.ReadAt)
                .HasColumnName("read_at");

            builder.HasOne(n => n.Tenant)
                .WithMany(t => t.Notifications)
                .HasForeignKey(n => n.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Recipient)
                .WithMany()
                .HasForeignKey(n => n.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
