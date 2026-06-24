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
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");

            builder.HasKey(rt => rt.RefreshTokenId);

            builder.Property(rt => rt.RefreshTokenId)
                .HasColumnName("refresh_column_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(rt => rt.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(rt => rt.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(rt => rt.Token)
                .HasColumnName("token")
                .IsRequired();

            builder.HasIndex(rt => rt.Token)
                .IsUnique();

            builder.Property(rt => rt.ExpiresAt)
                .HasColumnName("expires_at")
                .IsRequired();

            builder.Property(rt => rt.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(rt => rt.IsRevoked)
                .HasColumnName("is_revoked")
                .HasDefaultValue(false);

            builder.Property(rt => rt.RevokedAt)
                .HasColumnName("revoked_at");

            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rt => rt.Tenant)
                .WithMany()
                .HasForeignKey(rt => rt.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
