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
    public class PrepListItemConfiguration : IEntityTypeConfiguration<PrepListItem>
    {

        public void Configure(EntityTypeBuilder<PrepListItem> builder)
        {
            builder.ToTable("prep_list_items");

            builder.HasKey(pli => pli.PrepListItemId);

            builder.Property(pli => pli.PrepListItemId)
                .HasColumnName("prep_list_item_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(pli => pli.PrepListId)
                .HasColumnName("prep_list_id")
                .IsRequired();

            builder.Property(pli => pli.SourceType)
                .HasColumnName("source_type")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(pli => pli.ItemName)
                .HasColumnName("item_name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(pli => pli.RecipeId)
                .HasColumnName("recipe_id")
                .IsRequired();

            builder.Property(pli => pli.ScalingFactor)
                .HasColumnName("scaling_factor")
                .HasPrecision(10, 4);

            builder.Property(pli => pli.AnchorIngredientId)
                .HasColumnName("anchor_ingredient_id");

            builder.Property(pli => pli.AnchorQuantity)
                .HasColumnName("anchor_quantity")
                .HasPrecision(10, 4);

            builder.Property(pli => pli.Quantity)
                .HasColumnName("quantity")
                .HasPrecision(10, 4);

            builder.Property(pli => pli.Unit)
                .HasColumnName("unit")
                .HasMaxLength(50);

            builder.Property(pli => pli.Notes)
                .HasColumnName("notes");

            builder.Property(pli => pli.DisplayOrder)
                .HasColumnName("display_order")
                .IsRequired();

            builder.Property(pli => pli.IsComplete)
                .HasColumnName("is_complete")
                .HasDefaultValue(false);

            builder.Property(pli => pli.CompletedBy)
                .HasColumnName("completed_by");

            builder.Property(pli => pli.CompletedAt)
                .HasColumnName("completed_at");

            // keys & restraints

            builder.HasOne(pli => pli.PrepList)
                .WithMany(pl => pl.Items)
                .HasForeignKey(pli => pli.PrepListId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pli => pli.Recipe)
                .WithMany()
                .HasForeignKey(pli => pli.RecipeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(pli => pli.AnchorIngredient)
                .WithMany()
                .HasForeignKey(pli => pli.AnchorIngredientId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(pli => pli.CompletedByUser)
                .WithMany()
                .HasForeignKey(pli => pli.CompletedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
