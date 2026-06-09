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
    public class NotificationQueueConfiguration : IEntityTypeConfiguration<NotificationQueue>
    {
        public void Configure(EntityTypeBuilder<NotificationQueue> builder)
        {
            builder.ToTable("notification_queue");

            builder.HasKey(nq =>nq.QueueId);

            builder.Property(nq => nq.QueueId)
                .HasColumnName("queue_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(nq => nq.TenantId)
                .HasColumnName("teantn_id")
                .IsRequired();

            builder.Property(nq => nq.RecipientId)
                .HasColumnName("recipient_id")
                .IsRequired();

            builder.Property(nq => nq.NotificationId)
                .HasColumnName("notification_id")
                .IsRequired();

            builder.Property(nq => nq.QueuedAt)
                .HasColumnName("queued_at")
                .IsRequired();

            builder.Property(nq => nq.DeliveredAt)
                .HasColumnName("delivered_at");

            builder.Property(nq => nq.IsDelivered)
                .HasColumnName("is_delivered")
                .HasDefaultValue(false);

            builder.HasOne(nq => nq.Tenant)
                .WithMany()
                .HasForeignKey(nq => nq.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(nq => nq.Recipient)
                .WithMany()
                .HasForeignKey(nq => nq.RecipientId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(nq => nq.Notification)
                .WithMany(n => n.QueueEntries)
                .HasForeignKey(nq => nq.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
