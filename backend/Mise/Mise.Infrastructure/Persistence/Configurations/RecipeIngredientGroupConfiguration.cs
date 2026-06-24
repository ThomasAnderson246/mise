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
    public class RecipeIngredientGroupConfiguration : IEntityTypeConfiguration<RecipeIngredientGroup>
    {
        public void Configure(EntityTypeBuilder<RecipeIngredientGroup> builder)
        {
            builder.ToTable("recipe_ingredient_groups");

            builder.HasKey(g => g.GroupId);

            builder.Property(g => g.GroupId)
                .HasColumnName("group_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(g => g.VersionId)
                .HasColumnName("version_id")
                .IsRequired();

            builder.Property(g => g.Name)
                .HasColumnName(@"Name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(g => g.DisplayOrder)
                .HasColumnName("display_order")
                .IsRequired();

            builder.HasOne(g => g.Version)
                .WithMany(rv => rv.IngredientGroups)
                .HasForeignKey(g => g.VersionId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
