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
    public class StepCheckOffConfiguration : IEntityTypeConfiguration<StepCheckOff>
    {
        public void Configure(EntityTypeBuilder<StepCheckOff> builder)
        {
            builder.ToTable("step_check_offs");

            builder.HasKey(sc => sc.CheckOffId);

            builder.Property(sc => sc.CheckOffId)
                .HasColumnName("check_off_Id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(sc => sc.SessionId)
                .HasColumnName("session_id")
                .IsRequired();

            builder.Property(sc => sc.StepId)
                .HasColumnName("step_id")
                .IsRequired();

            builder.Property(sc => sc.CheckedBy)
                .HasColumnName("checked_by");

            builder.Property(sc => sc.CheckedAt)
                .HasColumnName("checked_at")
                .IsRequired();

            builder.Property(sc => sc.IsComplete)
                .HasColumnName("is_complete")
                .HasDefaultValue(true);

            builder.HasOne(sc => sc.Session)
                .WithMany(cs => cs.StepCheckOffs)
                .HasForeignKey(sc => sc.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sc => sc.Step)
                .WithMany(rs => rs.StepCheckOffs)
                .HasForeignKey(sc => sc.StepId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sc => sc.CheckedByUser)
                .WithMany()
                .HasForeignKey(sc => sc.CheckedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
