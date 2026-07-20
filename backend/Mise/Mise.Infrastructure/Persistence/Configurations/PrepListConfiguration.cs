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
    public class PrepListConfiguration : IEntityTypeConfiguration<PrepList>
    {
        public void Configure(EntityTypeBuilder<PrepList> builder)
        {
            builder.ToTable("prep_lists");

            builder.HasKey(pl => pl.PrepListId);

            builder.Property(pl => pl.PrepListId)
                .HasColumnName("prep_list_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(pl => pl.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(pl => pl.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(pl => pl.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(pl => pl.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(pl => pl.CompletedAt)
                .HasColumnName("completed_at");

            builder.Property(pl => pl.IsComplete)
                .HasColumnName("is_complete")
                .HasDefaultValue(false);

            builder.Property(pl => pl.AssignedTo)
                .HasColumnName("assigned_to");

            builder.HasOne(pl => pl.Tenant)
                .WithMany(t => t.PrepLists)
                .HasForeignKey(pl => pl.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pl => pl.CreatedByUser)
                .WithMany()
                .HasForeignKey(pl => pl.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(pl => pl.AssignedToUser)
                .WithMany()
                .HasForeignKey(pl => pl.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
