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
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("user_roles");

            builder.HasKey(ur => ur.UserRoleId);

            builder.Property(ur => ur.UserRoleId)
                .HasColumnName("user_role_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(ur => ur.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(ur => ur.RoleId)
                .HasColumnName("role_id")
                .IsRequired();

            builder.Property(ur => ur.AssignedAt)
                .HasColumnName("assigned_at")
                .IsRequired();

            builder.Property(ur => ur.AssignedBy)
                .HasColumnName("assigned_by");

            builder.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.AssignedByUser)
                .WithMany()
                .HasForeignKey(ur => ur.AssignedBy)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
