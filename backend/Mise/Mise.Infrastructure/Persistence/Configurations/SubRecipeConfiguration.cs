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
    public class SubRecipeConfiguration : IEntityTypeConfiguration<SubRecipe>
    {
        public void Configure(EntityTypeBuilder<SubRecipe> builder)
        {
            builder.ToTable("sub_recipes");

            builder.HasKey(sr => new {sr.ParentRecipeId, sr.SubRecipeId});

            builder.Property(sr => sr.ParentRecipeId)
                .HasColumnName("parent_recipe_id");

            builder.Property(sr => sr.SubRecipeId)
                .HasColumnName("sub_recipe_id");

            builder.ToTable(t => t.HasCheckConstraint(
                "chk_no_self_reference",
                "parent_recipe_id != sub_recipe_id"));

            builder.HasOne(sr => sr.ParentRecipe)
                .WithMany(r => r.ParentRecipes)
                .HasForeignKey(sr => sr.ParentRecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sr => sr.ChildRecipe)
                .WithMany(r => r.ChildRecipes)
                .HasForeignKey(sr => sr.SubRecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
