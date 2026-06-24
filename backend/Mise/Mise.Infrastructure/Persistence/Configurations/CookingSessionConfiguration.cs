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
    public class CookingSessionConfiguration : IEntityTypeConfiguration<CookingSession>
    {

        public void Configure(EntityTypeBuilder<CookingSession> builder)
        {
            builder.ToTable("cooking_sessions");

            builder.HasKey(cs => cs.SessionId);

            builder.Property(cs => cs.SessionId)
                .HasColumnName("session_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(cs => cs.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(cs => cs.RecipeId)
                .HasColumnName("recipe_id")
                .IsRequired();

            builder.Property(cs => cs.VersionId)
                .HasColumnName("version_id")
                .IsRequired();

            builder.Property(cs => cs.StartedAt)
                .HasColumnName("started_at")
                .IsRequired();

            builder.Property(cs => cs.StartedBy)
                .HasColumnName("started_by");

            builder.Property(cs => cs.CompletedAt)
                .HasColumnName("completed_at");

            builder.Property(cs => cs.IsComplete)
                .HasColumnName("is_complete")
                .HasDefaultValue(false);

            builder.HasOne(cs => cs.Tenant)
                .WithMany()
                .HasForeignKey(cs => cs.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cs => cs.Recipe)
                .WithMany()
                .HasForeignKey(cs => cs.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cs => cs.Version)
                .WithMany()
                .HasForeignKey(cs => cs.VersionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cs => cs.StartedByUser)
                .WithMany()
                .HasForeignKey(cs => cs.StartedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
