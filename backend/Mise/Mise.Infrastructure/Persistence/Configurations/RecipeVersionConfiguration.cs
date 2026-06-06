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
    public class RecipeVersionConfiguration : IEntityTypeConfiguration<RecipeVersion>
    {
        public void Configure(EntityTypeBuilder<RecipeVersion> builder)
        {
            builder.ToTable("recipe_versions");

            builder.HasKey(rv => rv.VersionId);

            builder.Property(rv => rv.VersionId)
                .HasColumnName("version_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(rv => rv.RecipeId)
                .HasColumnName("recipe_id")
                .IsRequired();

            builder.Property(rv => rv.VersionNumber)
                .HasColumnName("version_number")
                .IsRequired();

            builder.Property(rv => rv.IsDraft)
                .HasColumnName("is_draft")
                .HasDefaultValue(true);

            builder.Property(rv => rv.IsPublished)
                .HasColumnName("is_published")
                .HasDefaultValue(false);

            builder.Property(rv => rv.PublishedBy)
                .HasColumnName("published_by");

            builder.Property(rv => rv.PublishedAt)
                .HasColumnName("published_at");

            builder.Property(rv => rv.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.HasIndex(rv => new { rv.RecipeId, rv.VersionNumber })
                .IsUnique();

            builder.HasOne(rv => rv.Recipe)
                .WithMany(r => r.Versions)
                .HasForeignKey(rv => rv.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rv => rv.PublishedByUser)
                .WithMany()
                .HasForeignKey(rv => rv.PublishedBy)
                .OnDelete(DeleteBehavior.SetNull);
              
        }
    }
}
