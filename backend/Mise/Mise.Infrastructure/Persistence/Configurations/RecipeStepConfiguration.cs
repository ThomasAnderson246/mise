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
    public class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
    {
        public void Configure(EntityTypeBuilder<RecipeStep> builder)
        {
            builder.ToTable("recipe_steps");

            builder.HasKey(rs => rs.StepId);

            builder.Property(rs => rs.StepId)
                .HasColumnName("step_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(rs => rs.VersionId)
                .HasColumnName("version_id")
                .IsRequired();

            builder.Property(rs => rs.StepNumber)
                .HasColumnName("step_number")
                .IsRequired();

            builder.Property(rs => rs.Instruction)
                .HasColumnName("instruction")
                .IsRequired();

            builder.Property(rs => rs.IsAsync)
                .HasColumnName("is_async")
                .HasDefaultValue(false);

            builder.Property(rs => rs.AsyncGroupId)
                .HasColumnName("async_group_id");

            builder.Property(rs => rs.HasTimer)
                .HasColumnName("has_timer")
                .HasDefaultValue(false);

            builder.Property(rs => rs.TimerDuration)
                .HasColumnName("timer_duration");

            builder.Property(rs => rs.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.HasOne(rs => rs.Version)
                .WithMany(rv => rv.Steps)
                .HasForeignKey(rs => rs.VersionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
