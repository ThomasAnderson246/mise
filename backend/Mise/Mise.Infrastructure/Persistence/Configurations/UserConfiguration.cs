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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.UserId).HasColumnName("user_id").HasDefaultValueSql("gen_random_uuid()");

            builder.Property(u => u.TenantId).HasColumnName("tenant_id").IsRequired();

            builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();

            builder.Property(u => u.PasswordHash).HasColumnName("password_hash");

            builder.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();

            builder.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();

            builder.Property(u => u.Status).HasColumnName("status").HasMaxLength(10).IsRequired();

            builder.Property(u => u.UnitPreference).HasColumnName("unit_preference").HasMaxLength(10).IsRequired();

            builder.Property(u => u.LastLoginAt).HasColumnName("last_login_at");

            builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();

            builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").IsRequired();

            // constraints
            builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();

            // relationships
            builder.HasOne( u => u.Tenant).WithMany(t => t.Users).HasForeignKey(u => u.TenantId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
