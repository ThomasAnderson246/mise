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
    public class IngredientCheckOffConfiguration : IEntityTypeConfiguration<IngredientCheckOff>
    {
        public void Configure(EntityTypeBuilder<IngredientCheckOff> builder)
        {
            builder.ToTable("ingredient_check_offs");

            builder.HasKey(ic => ic.CheckOffId);

            builder.Property(ic => ic.CheckOffId)
                .HasColumnName("check_off_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(ic => ic.SessionId)
                .HasColumnName("session_id")
                .IsRequired();

            builder.Property(ic => ic.RecipeIngredientId)
                .HasColumnName("recipe_ingredient_id")
                .IsRequired();

            builder.Property(ic => ic.CheckedBy)
                .HasColumnName("checked_by");

            builder.Property(ic => ic.CheckedAt)
                .HasColumnName("checked_at")
                .IsRequired();

            builder.Property(ic => ic.IsComplete)
                .HasColumnName("is_complete")
                .HasDefaultValue(true);

            builder.HasOne(ic => ic.Session)
                .WithMany(cs => cs.IngredientCheckOffs)
                .HasForeignKey(ic => ic.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ic => ic.RecipeIngredient)
                .WithMany(ri => ri.IngredientCheckOffs)
                .HasForeignKey(ic => ic.RecipeIngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ic => ic.CheckedByUser)
                .WithMany()
                .HasForeignKey(ic => ic.CheckedBy)
                .OnDelete(DeleteBehavior.SetNull);


        }
    }
}
